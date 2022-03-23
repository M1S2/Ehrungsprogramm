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

        public const string BOARD_MEMBER_MARKER = "Vorstand";
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
            string[] csv_lines = System.IO.File.ReadAllLines(filepath, Encoding.GetEncoding("iso-8859-1")); //Encoding.UTF8);

            string regexPatternLineElements = DELIMITER + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";       //Using quotes to allow the delimiter

            DateTime dateTimeNow = DateTime.Now;        // Get the current time once to always have the same value (otherwise Equals could fail)

            int current_line_index = 0;
            foreach (string line in csv_lines)
            {
                cancellationToken.ThrowIfCancellationRequested();

                current_line_index++;

                // if it's the header line, continue with the next line
                if (line.StartsWith(HEADERLINE_START_STRING)) { continue; }

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
                    person.BirthDate = DateTime.Parse(line_split[1].Trim('"').Trim());
                }

                // The third column contains the entry date
                if (line_split.Length >= 3)
                {
                    person.EntryDate = DateTime.Parse(line_split[2].Trim('"').Trim());
                }

                person.Functions = new List<Function>();
                for (int functionBlockStartIndex = FIRST_FUNCTIONBLOCK_INDEX; (functionBlockStartIndex + (FUNCTIONBLOCK_COLUMNS + REWARDBLOCK_COLUMNS) - 1) < line_split.Length; functionBlockStartIndex += (FUNCTIONBLOCK_COLUMNS + REWARDBLOCK_COLUMNS))
                {
                    string entryDate = line_split[functionBlockStartIndex].Trim('"').Trim();
                    string functionName = line_split[functionBlockStartIndex + 1].Trim('"').Trim();
                    string functionStartDate = line_split[functionBlockStartIndex + 2].Trim('"').Trim();
                    string functionEndDate = line_split[functionBlockStartIndex + 3].Trim('"').Trim();

                    // if the function name is empty, the function block isn't valid.
                    if (string.IsNullOrEmpty(functionName)) { break; }

                    Function function = new Function();
                    function.Description = functionName;
                    if (!string.IsNullOrEmpty(functionStartDate)) { function.TimePeriod.Start = DateTime.Parse(functionStartDate); }
                    else { person.ParsingFailureMessage = String.Format("Function Start Date empty (Function: {0})!", functionName); }
                    
                    if (!string.IsNullOrEmpty(functionEndDate)) 
                    {
                        function.IsFunctionOngoing = false;
                        function.TimePeriod.End = DateTime.Parse(functionEndDate);
                    }
                    else    // if the end date column is empty, the function is not ended yet (lasts until now)
                    {
                        function.IsFunctionOngoing = true;
                        function.TimePeriod.End = dateTimeNow;      // This date is replaced later by the calculation deadline. So it doesn't matter what it is set to.
                    }

                    if (functionName.Contains(OTHER_FUNCTIONS_MARKER)) { function.Type = FunctionType.OTHER_FUNCTION; }
                    else if (functionName.Contains(HEAD_OF_DEPARTEMENT_MARKER)) { function.Type = FunctionType.HEAD_OF_DEPARTEMENT; }
                    else if (functionName.Contains(BOARD_MEMBER_MARKER)) { function.Type = FunctionType.BOARD_MEMBER; }
                    else { function.Type = FunctionType.UNKNOWN; }

                    if (!person.Functions.Contains(function))
                    {
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
                    if (!string.IsNullOrEmpty(rewardDate)) { reward.ObtainedDate = DateTime.Parse(rewardDate); }
                    switch (int.Parse(rewardNumber))
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

                    if(!person.Rewards.AddReward(reward))
                    {
                        // Unknown reward type
                    }
                }

                people.Add(person);

                OnParseProgress?.Invoke(filepath, (current_line_index / (float)csv_lines.Length) * 100);
            }

            return people;
        }
    }
}
