using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Ehrungsprogramm.Core.Models;
using Ehrungsprogramm.Core.Contracts.Services;

namespace Ehrungsprogramm.Core.Services
{
    public static class CsvFileParserProWinner
    {
        /// <summary>
        /// Event that is raised when the import progress changes
        /// </summary>
        public static event ProgressDelegate OnParseProgress;

        public const string DELIMITER = ";";
        public const string HEADERLINE_START_STRING = "\"Name, Vorname\"";
        public const int FIRST_FUNCTIONBLOCK_INDEX = 2;
        public const int FUNCTIONBLOCK_COLUMNS = 4;
        public const int FIRST_REWARDBLOCK_INDEX = 6;
        public const int REWARDBLOCK_COLUMNS = 3;

        public const string BOARD_MEMBER_MARKER = "1. 2. 3. Vorstand";
        public const string BOARD_MEMBER_FUNCTION_MARKER = "Vorstandschaft";        // someone who has another function than 1. - 3. board member but is in the board too.
        public const string HEAD_OF_DEPARTEMENT_MARKER = "Abteilungsleiter";
        public const string OTHER_FUNCTIONS_MARKER = "FKT";

        public const int REWARD_NUMBER_BLSV20 = 14;
        public const int REWARD_NUMBER_BLSV25 = 15;
        public const int REWARD_NUMBER_BLSV30 = 16;
        public const int REWARD_NUMBER_BLSV40 = 17;
        public const int REWARD_NUMBER_BLSV45 = 71;
        public const int REWARD_NUMBER_BLSV50 = 18;
        public const int REWARD_NUMBER_BLSV60 = 20;
        public const int REWARD_NUMBER_BLSV70 = 22;
        public const int REWARD_NUMBER_BLSV80 = 72;
        public const int REWARD_NUMBER_TSV_SILVER = 3;
        public const int REWARD_NUMBER_TSV_GOLD = 4;
        public const int REWARD_NUMBER_TSV_HONORARY = 1;

        // Header line format:
        //                             | FUNCTION 0                               | REWARD 0                                | FUNCTION 1                                                | REWARD 1                             | ...
        // "Name, Vorname";"Geb.Datum";"Eintritt am";"Funktionsname";"von -";"bis";"Ehr.Nr.";"Ehr.dat.";"Ehrungsbezeichnung";"Eintritt am";"Funktionsname";"Funktion von";"Funktion bis";"Ehrungs-Nr";"Ehrung am";"Ehrungsname"; ...

        public static List<Person> Parse(string filepath, CancellationToken cancellationToken)
        {
            List<Person> people = new List<Person>();

            // Check FilePath
            if (!File.Exists(filepath))
            {
                return people;
            }
            if (Path.GetExtension(filepath).ToLower() != ".txt" && Path.GetExtension(filepath).ToLower() != ".csv")
            {
                return people;
            }

            // Read all lines of the .csv file
            Encoding fileEncoding = getFileEncoding(filepath);
            string[] csv_lines = System.IO.File.ReadAllLines(filepath, fileEncoding);

            string regexPatternLineElements = DELIMITER + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";       //Using quotes to allow the delimiter

            DateTime dateTimeNow = DateTime.Now;        // Get the current time once to always have the same value (otherwise Equals could fail)

            int current_line_index = 0;
            foreach (string line in csv_lines)
            {
                cancellationToken.ThrowIfCancellationRequested();

                current_line_index++;

                // if it's the header line, continue with the next line
                if (line.StartsWith(HEADERLINE_START_STRING)) { continue; }

                StringBuilder person_errors = new StringBuilder();
                Person person = new Person();
                string[] line_split = Regex.Split(line, regexPatternLineElements);                  //split line (using quotes to allow the delimiter)

                // The first column contains the name and first name in the format "<Name>, <First Name>"
                if (line_split.Length >= 1)
                {
                    string[] nameSplit = line_split[0].Trim('"').Split(',');
                    person.Name = nameSplit[0].Trim();
                    person.FirstName = nameSplit[1].Trim();
                }

                // The second column contains the birth date
                if (line_split.Length >= 2)
                {
                    DateTime birthDate;
                    if (DateTime.TryParse(line_split[1].Trim('"').Trim(), out birthDate))
                    {
                        person.BirthDate = birthDate;
                    }
                    else { person_errors.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserBirthDate, line_split[1].Trim('"').Trim())); }
                }

                // The third column contains the entry date
                if (line_split.Length >= 3)
                {
                    DateTime entryDate;
                    if (DateTime.TryParse(line_split[2].Trim('"').Trim(), out entryDate))
                    {
                        person.EntryDate = entryDate;
                    }
                    else { person_errors.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserEntryDate, line_split[2].Trim('"').Trim())); }
                }

                person.Functions = new List<Function>();
                for (int functionBlockStartIndex = FIRST_FUNCTIONBLOCK_INDEX; (functionBlockStartIndex + (FUNCTIONBLOCK_COLUMNS + REWARDBLOCK_COLUMNS) - 1) < line_split.Length; functionBlockStartIndex += (FUNCTIONBLOCK_COLUMNS + REWARDBLOCK_COLUMNS))
                {
                    StringBuilder function_error = new StringBuilder();

                    string entryDate = line_split[functionBlockStartIndex].Trim('"').Trim();
                    string functionName = line_split[functionBlockStartIndex + 1].Trim('"').Trim();
                    string functionStartDate = line_split[functionBlockStartIndex + 2].Trim('"').Trim();
                    string functionEndDate = line_split[functionBlockStartIndex + 3].Trim('"').Trim();

                    // if the function name is empty, the function block isn't valid.
                    if (string.IsNullOrEmpty(functionName)) { break; }

                    Function function = new Function();
                    function.Description = functionName;

                    DateTime parsedFunctionStartDate;
                    if (DateTime.TryParse(functionStartDate, out parsedFunctionStartDate))
                    {
                        function.TimePeriod.Start = parsedFunctionStartDate;
                    }
                    else { function_error.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserFunctionStartDate, functionStartDate, functionName)); }
                    
                    if (!string.IsNullOrEmpty(functionEndDate)) 
                    {
                        function.IsFunctionOngoing = false;

                        DateTime parsedFunctionEndDate;
                        if (DateTime.TryParse(functionEndDate, out parsedFunctionEndDate))
                        {
                            function.TimePeriod.End = parsedFunctionEndDate;
                        }
                        else { function_error.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserFunctionEndDate, functionEndDate, functionName)); }
                    }
                    else    // if the end date column is empty, the function is not ended yet (lasts until now)
                    {
                        function.IsFunctionOngoing = true;
                        function.TimePeriod.End = dateTimeNow;      // This date is replaced later by the calculation deadline. So it doesn't matter what it is set to.
                    }

                    if (functionName.Contains(OTHER_FUNCTIONS_MARKER)) { function.Type = FunctionType.OTHER_FUNCTION; }
                    else if (functionName.Contains(HEAD_OF_DEPARTEMENT_MARKER) || functionName.Contains(BOARD_MEMBER_FUNCTION_MARKER)) { function.Type = FunctionType.HEAD_OF_DEPARTEMENT; }
                    else if (functionName.Contains(BOARD_MEMBER_MARKER)) { function.Type = FunctionType.BOARD_MEMBER; }
                    else { function.Type = FunctionType.UNKNOWN; }

                    if (!person.Functions.Contains(function))
                    {
                        if (function_error.Length > 0) { person_errors.Append(function_error.ToString()); }
                        person.Functions.Add(function);
                    }
                }

                for (int rewardBlockStartIndex = FIRST_REWARDBLOCK_INDEX; (rewardBlockStartIndex + REWARDBLOCK_COLUMNS - 1) < line_split.Length; rewardBlockStartIndex += (FUNCTIONBLOCK_COLUMNS + REWARDBLOCK_COLUMNS))
                {
                    string rewardNumber = line_split[rewardBlockStartIndex].Trim('"').Trim();
                    string rewardDate = line_split[rewardBlockStartIndex + 1].Trim('"').Trim();
                    string rewardDescription = line_split[rewardBlockStartIndex + 2].Trim('"').Trim();

                    // if the reward description is empty, the reward block isn't valid.
                    if (string.IsNullOrEmpty(rewardDescription)) { break; }

                    Reward reward = new Reward();
                    reward.Description = rewardDescription;
                    reward.Obtained = true;
                    reward.Available = true;

                    DateTime parsedRewardObtainedDate;
                    if (DateTime.TryParse(rewardDate, out parsedRewardObtainedDate))
                    {
                        reward.ObtainedDate = parsedRewardObtainedDate;
                    }
                    else { person_errors.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserRewardObtainedDate, rewardDate, rewardDescription)); }

                    int parsedRewardNumber;
                    if (int.TryParse(rewardNumber, out parsedRewardNumber))
                    {
                        switch (parsedRewardNumber)
                        {
                            case REWARD_NUMBER_BLSV20: reward.Type = RewardTypes.BLSV20; break;
                            case REWARD_NUMBER_BLSV25: reward.Type = RewardTypes.BLSV25; break;
                            case REWARD_NUMBER_BLSV30: reward.Type = RewardTypes.BLSV30; break;
                            case REWARD_NUMBER_BLSV40: reward.Type = RewardTypes.BLSV40; break;
                            case REWARD_NUMBER_BLSV45: reward.Type = RewardTypes.BLSV45; break;
                            case REWARD_NUMBER_BLSV50: reward.Type = RewardTypes.BLSV50; break;
                            case REWARD_NUMBER_BLSV60: reward.Type = RewardTypes.BLSV60; break;
                            case REWARD_NUMBER_BLSV70: reward.Type = RewardTypes.BLSV70; break;
                            case REWARD_NUMBER_BLSV80: reward.Type = RewardTypes.BLSV80; break;
                            case REWARD_NUMBER_TSV_SILVER: reward.Type = RewardTypes.TSVSILVER; break;
                            case REWARD_NUMBER_TSV_GOLD: reward.Type = RewardTypes.TSVGOLD; break;
                            case REWARD_NUMBER_TSV_HONORARY: reward.Type = RewardTypes.TSVHONORARY; break;
                            default: reward.Type = RewardTypes.UNKNOWN; break;
                        }

                        if (!person.Rewards.AddReward(reward))
                        {
                            // Unknown reward type
                        }
                    }
                    else { person_errors.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserRewardNumber, rewardNumber, rewardDescription)); }
                }

                person.ParsingFailureMessage = person_errors.ToString();
                people.Add(person);

                OnParseProgress?.Invoke(filepath, (current_line_index / (float)csv_lines.Length) * 100);
            }

            return people;
        }

        /// <summary>
        /// Using encoding from BOM or UTF8 if no BOM found, check if the file is valid, by reading all lines
        /// If decoding fails, use the Latin1 codepage
        /// </summary>
        /// <param name="filename">file to check</param>
        /// <returns>Detected Encoding</returns>
        /// https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        private static Encoding getFileEncoding(string filename)
        {
            Encoding Utf8EncodingVerifier = Encoding.GetEncoding("utf-8", new EncoderExceptionFallback(), new DecoderExceptionFallback());
            using (var reader = new StreamReader(filename, Utf8EncodingVerifier, true))
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                    }
                    return reader.CurrentEncoding;
                }
                catch (Exception)
                {
                    // Failed to decode the file using the BOM/UT8. Assume it's Latin1
                    return Encoding.Latin1;
                }
            }
        }

    }
}
