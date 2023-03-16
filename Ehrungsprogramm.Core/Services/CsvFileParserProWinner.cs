using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Ehrungsprogramm.Core.Models;
using Ehrungsprogramm.Core.Contracts.Services;
using System.Linq;

namespace Ehrungsprogramm.Core.Services
{
    /// <summary>
    /// Class used to import a CSV file with person data exported from ProWinner club software
    /// </summary>
    public static class CsvFileParserProWinner
    {
        /// <summary>
        /// Event that is raised when the import progress changes
        /// </summary>
        public static event ProgressDelegate OnParseProgress;

        /// <summary>
        /// Delimiter used in the CSV file
        /// </summary>
        public const string DELIMITER = ";";
        
        // Lists used to identify the columns
        public static readonly List<string> CSVCOLUMNHEADERS_NAME = new List<string>() { "Name, Vorname" };
        public static readonly List<string> CSVCOLUMNHEADERS_BIRTHDATE = new List<string>() { "Geb.Datum" };
        public static readonly List<string> CSVCOLUMNHEADERS_ENTRYDATE = new List<string>() { "Eintritt am" };
        public static readonly List<string> CSVCOLUMNHEADERS_FUNCTION_DESCRIPTION = new List<string>() { "Funktionsname" };
        public static readonly List<string> CSVCOLUMNHEADERS_FUNCTION_START = new List<string>() { "von -", "Funktion von" };
        public static readonly List<string> CSVCOLUMNHEADERS_FUNCTION_END = new List<string>() { "bis", "Funktion bis" };
        public static readonly List<string> CSVCOLUMNHEADERS_REWARD_NUMBER = new List<string>() { "Ehr.Nr.", "Ehrungs-Nr" };
        public static readonly List<string> CSVCOLUMNHEADERS_REWARD_DATE = new List<string>() { "Ehr.dat.", "Ehrung am" };
        public static readonly List<string> CSVCOLUMNHEADERS_REWARD_DESCRIPTION = new List<string>() { "Ehrungsbezeichnung", "Ehrungsname" };

        // Example header line format (columns can be ordered different):
        // "Name, Vorname";"Geb.Datum";"Eintritt am";"Funktionsname";"von -";"bis";"Ehr.Nr.";"Ehr.dat.";"Ehrungsbezeichnung";"Eintritt am";"Funktionsname";"Funktion von";"Funktion bis";"Ehrungs-Nr";"Ehrung am";"Ehrungsname"; ...
        //                             | FUNCTION 0                               | REWARD 0                                | FUNCTION 1                                                | REWARD 1                             | ...

        public const string BOARD_MEMBER_MARKER = "1. 2. 3. Vorstand";
        public const string BOARD_MEMBER_FUNCTION_MARKER = "Vorstandschaft";        // someone who has another function than 1. - 3. board member but is in the board too.
        public const string BOARD_MEMBER_FUNCTION_MARKER2 = "HV-";                   // alternative marker for the BOARD_MEMBER_FUNCTION_MARKER
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

        /// <summary>
        /// Parse the given CSV file to a list of <see cref="Person"/> objects
        /// </summary>
        /// <param name="filepath">Path to the CSV file</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> object used to cancel the parse operation</param>
        /// <returns>List of parses <see cref="Person"/> objects</returns>
        /// <exception cref="Exception"></exception>
        public static List<Person> Parse(string filepath, CancellationToken cancellationToken)
        {
            List<Person> people = new List<Person>();

            // Check FilePath
            if (!File.Exists(filepath))
            {
                throw new Exception(String.Format(Properties.Resources.ErrorCsvFileParserFileDoesntExist, filepath));
            }

            string[] csv_lines = null;
            try
            {
                // Read all lines of the .csv file
                Encoding fileEncoding = getFileEncoding(filepath);
                csv_lines = File.ReadAllLines(filepath, fileEncoding);
            }
            catch(IOException)
            {
                throw new Exception(string.Format(Properties.Resources.ErrorCsvFileParserFileOpenFailed, filepath));
            }

            string regexPatternLineElements = DELIMITER + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";       // Using quotes to allow the delimiter

            DateTime dateTimeNow = DateTime.Now;        // Get the current time once to always have the same value (otherwise Equals could fail)

            // Indices of the found columns
            int columnIndex_Name = -1;
            int columnIndex_BirthDate = -1;
            int columnIndex_EntryDate = -1;
            List<int> columnIndices_Function_Description = new List<int>();
            List<int> columnIndices_Function_Start = new List<int>();
            List<int> columnIndices_Function_End = new List<int>();
            List<int> columnIndices_Reward_Number = new List<int>();
            List<int> columnIndices_Reward_Date = new List<int>();
            List<int> columnIndices_Reward_Description = new List<int>();

            // Loop all CSV file lines
            int current_line_index = 0;
            foreach (string line in csv_lines)
            {
                cancellationToken.ThrowIfCancellationRequested();

                List<string> line_split = Regex.Split(line, regexPatternLineElements).ToList();                 // split line (using quotes to allow the delimiter)
                line_split = line_split.Select(element => element = element.Trim('"').Trim()).ToList();         // remove all surrounding quotes from the line elements

                current_line_index++;

                // Get all column indices from the first line (header line) and continue to the next line
                if (current_line_index == 1) 
                {
                    var indexed_line_split_elements = line_split.Select((s, i) => new { element = s, index = i });      // create a helper list with indices for each element

                    columnIndex_Name = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_NAME.Contains(list.element)).Select(list => list.index).FirstOrDefault(-1);
                    columnIndex_BirthDate = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_BIRTHDATE.Contains(list.element)).Select(list => list.index).FirstOrDefault(-1);
                    columnIndex_EntryDate = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_ENTRYDATE.Contains(list.element)).Select(list => list.index).FirstOrDefault(-1);
                    columnIndices_Function_Description = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_FUNCTION_DESCRIPTION.Contains(list.element)).Select(list => list.index).ToList();
                    columnIndices_Function_Start = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_FUNCTION_START.Contains(list.element)).Select(list => list.index).ToList();
                    columnIndices_Function_End = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_FUNCTION_END.Contains(list.element)).Select(list => list.index).ToList();
                    columnIndices_Reward_Number = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_REWARD_NUMBER.Contains(list.element)).Select(list => list.index).ToList();
                    columnIndices_Reward_Date = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_REWARD_DATE.Contains(list.element)).Select(list => list.index).ToList();
                    columnIndices_Reward_Description = indexed_line_split_elements.Where(list => CSVCOLUMNHEADERS_REWARD_DESCRIPTION.Contains(list.element)).Select(list => list.index).ToList();


                    // plausibility checks (columns must exist)
                    StringBuilder columnErrorsList = new StringBuilder();
                    if (columnIndex_Name < 0) { columnErrorsList.AppendLine(string.Format(Properties.Resources.ErrorCsvFileParserColumnMissing, CSVCOLUMNHEADERS_NAME.FirstOrDefault())); }
                    // BirthDate column is optional
                    if (columnIndex_EntryDate < 0) { columnErrorsList.AppendLine(string.Format(Properties.Resources.ErrorCsvFileParserColumnMissing, CSVCOLUMNHEADERS_ENTRYDATE.FirstOrDefault())); }
                    if (columnIndices_Function_Description.Count == 0 || columnIndices_Function_Start.Count == 0 || columnIndices_Function_End.Count == 0) 
                    {
                        columnErrorsList.AppendLine(string.Format(Properties.Resources.ErrorCsvFileParserColumnMissingFunctions, CSVCOLUMNHEADERS_FUNCTION_DESCRIPTION.FirstOrDefault(), CSVCOLUMNHEADERS_FUNCTION_START.FirstOrDefault(), CSVCOLUMNHEADERS_FUNCTION_END.FirstOrDefault()));
                    }
                    if (columnIndices_Reward_Number.Count == 0 || columnIndices_Reward_Date.Count == 0 || columnIndices_Reward_Description.Count == 0)
                    {
                        columnErrorsList.AppendLine(string.Format(Properties.Resources.ErrorCsvFileParserColumnMissingRewards, CSVCOLUMNHEADERS_REWARD_NUMBER.FirstOrDefault(), CSVCOLUMNHEADERS_REWARD_DATE.FirstOrDefault(), CSVCOLUMNHEADERS_REWARD_DESCRIPTION.FirstOrDefault()));
                    }

                    if (columnErrorsList.Length > 0) { throw new Exception(string.Format(Properties.Resources.ErrorCsvFileParserColumnErrors, columnErrorsList.ToString())); }

                    continue; 
                }

                StringBuilder person_errors = new StringBuilder();
                Person person = new Person();
                
                // Get the name and first name in the format "<Name>, <First Name>" from the corresponding column
                if (columnIndex_Name >= 0)
                {
                    string[] nameSplit = line_split[columnIndex_Name].Split(',');
                    person.Name = nameSplit[0].Trim();
                    person.FirstName = nameSplit[1].Trim();
                }
                else
                {
                    person.Name = string.Empty;
                    person.FirstName = string.Empty;
                }

                // Get the birth date from the corresponding column
                if (columnIndex_BirthDate >= 0)
                {
                    DateTime birthDate;
                    if (DateTime.TryParse(line_split[columnIndex_BirthDate], out birthDate))
                    {
                        person.BirthDate = birthDate;
                    }
                    else { person_errors.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserBirthDate, line_split[columnIndex_BirthDate])); }
                }

                // Get the entry date from the corresponding column
                if (columnIndex_EntryDate >= 0)
                {
                    DateTime entryDate;
                    if (DateTime.TryParse(line_split[columnIndex_EntryDate], out entryDate))
                    {
                        person.EntryDate = entryDate;
                    }
                    else { person_errors.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserEntryDate, line_split[columnIndex_EntryDate])); }
                }

                person.Functions = new List<Function>();
                for(int functionIndex = 0; functionIndex < Math.Min(columnIndices_Function_Description.Count, Math.Min(columnIndices_Function_Start.Count, columnIndices_Function_End.Count)); functionIndex++)
                {
                    StringBuilder function_error = new StringBuilder();

                    string functionName = line_split[columnIndices_Function_Description[functionIndex]];
                    string functionStartDate = line_split[columnIndices_Function_Start[functionIndex]];
                    string functionEndDate = line_split[columnIndices_Function_End[functionIndex]];

                    // if the function name is empty, the function block isn't valid.
                    if (string.IsNullOrEmpty(functionName)) { continue; }

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
                            if (parsedFunctionEndDate < parsedFunctionStartDate)
                            {
                                function_error.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserFunctionStartEndDateSwapped, functionStartDate, functionEndDate, functionName));
                            }
                            else
                            {
                                function.TimePeriod.End = parsedFunctionEndDate;
                            }
                        }
                        else { function_error.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserFunctionEndDate, functionEndDate, functionName)); }
                    }
                    else    // if the end date column is empty, the function is not ended yet (lasts until now)
                    {
                        function.IsFunctionOngoing = true;
                        function.TimePeriod.End = dateTimeNow;      // This date is replaced later by the calculation deadline. So it doesn't matter what it is set to.
                    }

                    // Check if the functionName contains some key words and assign the function.Type accordingly
                    if (functionName.Contains(OTHER_FUNCTIONS_MARKER)) 
                    { 
                        function.Type = FunctionType.OTHER_FUNCTION; 
                    }
                    else if (functionName.Contains(HEAD_OF_DEPARTEMENT_MARKER) || functionName.Contains(BOARD_MEMBER_FUNCTION_MARKER) || functionName.Contains(BOARD_MEMBER_FUNCTION_MARKER2)) 
                    { 
                        function.Type = FunctionType.HEAD_OF_DEPARTEMENT;
                    }
                    else if (functionName.Contains(BOARD_MEMBER_MARKER)) 
                    { 
                        function.Type = FunctionType.BOARD_MEMBER; 
                    }
                    else 
                    { 
                        function.Type = FunctionType.UNKNOWN; 
                    }

                    if (!person.Functions.Contains(function))
                    {
                        if (function_error.Length > 0) { person_errors.Append(function_error.ToString()); }
                        person.Functions.Add(function);
                    }
                }

                for(int rewardIndex = 0; rewardIndex < Math.Min(columnIndices_Reward_Number.Count, Math.Min(columnIndices_Reward_Date.Count, columnIndices_Reward_Description.Count)); rewardIndex++)
                {
                    string rewardNumber = line_split[columnIndices_Reward_Number[rewardIndex]];
                    string rewardDate = line_split[columnIndices_Reward_Date[rewardIndex]];
                    string rewardDescription = line_split[columnIndices_Reward_Description[rewardIndex]];

                    // if the reward description is empty, the reward block isn't valid.
                    if (string.IsNullOrEmpty(rewardDescription)) { continue; }

                    Reward reward = new Reward();
                    reward.Description = rewardDescription;
                    reward.Obtained = true;
                    reward.Available = true;

                    DateTime parsedRewardObtainedDate;
                    if (DateTime.TryParse(rewardDate, out parsedRewardObtainedDate))
                    {
                        reward.ObtainedDate = parsedRewardObtainedDate;
                    }
                    else if(!person.Rewards.Rewards.Contains(reward))   // Only report the obtained date error once for each reward
                    { 
                        person_errors.AppendLine(String.Format(Properties.Resources.ErrorCsvFileParserRewardObtainedDate, rewardDate, rewardDescription)); 
                    }

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
                            default: reward.Type = RewardTypes.OTHER; break;
                        }

                        if (!person.Rewards.AddReward(reward))
                        {
                            // Reward not added (same reward was already in collection)
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
