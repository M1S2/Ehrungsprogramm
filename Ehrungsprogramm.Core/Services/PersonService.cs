using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Core.Models;
using LiteDB;
using Itenso.TimePeriod;

namespace Ehrungsprogramm.Core.Services
{
    /// <summary>
    /// Service used to get and store a list of Person objects
    /// </summary>
    public class PersonService : IPersonService
    {
        /// <summary>
        /// Settings that are stored in the database beside the people
        /// </summary>
        public class PersonServiceSettings
        {
            public DateTime CalculationDeadline { get; set; }
            public string LastImportFilePath { get; set; }
        }



        /// <summary>
        /// Event that is raised when the import from the file is finished.
        /// </summary>
        public event EventHandler OnImportFromFileFinished;

        /// <summary>
        /// Event that is raised when the import progress changes
        /// </summary>
        public event ProgressDelegate OnImportFromFileProgress
        {
            add { CsvFileParserProWinner.OnParseProgress += value; }
            remove { CsvFileParserProWinner.OnParseProgress -= value; }
        }

        /// <summary>
        /// End date that is used while calculating the membership years and the years for each function
        /// </summary>
        public DateTime CalculationDeadline
        {
            get => _settingsCollection.Query().FirstOrDefault()?.CalculationDeadline ?? DateTime.Now;
            set
            {
                string lastImportFilePath = LastImportFilePath;
                _settingsCollection.DeleteAll();
                _settingsCollection.Insert(new PersonServiceSettings() { CalculationDeadline = value, LastImportFilePath = lastImportFilePath });
            }
        }


        /// <summary>
        /// Path of the last imported file
        /// </summary>
        public string LastImportFilePath
        {
            get => _settingsCollection.Query().FirstOrDefault()?.LastImportFilePath ?? "";
            set
            {
                DateTime calculationDeadline = CalculationDeadline;
                _settingsCollection.DeleteAll();
                _settingsCollection.Insert(new PersonServiceSettings() { LastImportFilePath = value, CalculationDeadline = calculationDeadline });
            }
        }

        private const int REWARD_TSVSILVER_POINTS = 45;
        private const int REWARD_TSVGOLD_POINTS = 70;
        private const int REWARD_TSVHONORARY_POINTS = 85;

        private LiteDatabase _database;                                     // Handle to a database holding the collection of Persons
        private ILiteCollection<Person> _peopleCollection;                  // Collection of Persons
        private ILiteCollection<PersonServiceSettings> _settingsCollection; // Collection of Settings

        /// <summary>
        /// Constructor that opens a database from file and gets all people from the database.
        /// </summary>
        public PersonService()
        {
            // TODO Replace fixed filepath for batabase by some kind of property
            _database = new LiteDatabase(@"S:\IT\Ehrungsprogramm\Ehrungsprogramm_Persons.db");
            _peopleCollection = _database.GetCollection<Person>("people");
            _settingsCollection = _database.GetCollection<PersonServiceSettings>("settings");
        }

        /// <summary>
        /// Import a list of Personsto an internal database.
        /// This is using a separate Task because the file possibly can be large.
        /// </summary>
        /// <param name="filepath">filepath to the file to import</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        public async Task<bool> ImportFromFile(string filepath, CancellationToken cancellationToken)
        {
            bool importingResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    List<Person> importedPeople = CsvFileParserProWinner.Parse(filepath, cancellationToken);
                    ClearPersons();
                    _peopleCollection?.InsertBulk(importedPeople);
                    importingResult = true;
                    LastImportFilePath = filepath;
                }
                catch(OperationCanceledException)
                {
                    importingResult = false;
                }
                catch(Exception ex)
                {
                    exception = ex;
                }
            });
            OnImportFromFileFinished?.Invoke(this, null);
            if(exception != null) { throw exception; }
            return importingResult;
        }

        /// <summary>
        /// Return all available Persons.
        /// </summary>
        /// <returns>List of <see cref="Person"/> objects</returns>
        public List<Person> GetPersons()
        {
            List<Person> people = _peopleCollection?.Query().ToList();
            people.ForEach(p => updatePersonProperties(p));
            return people;
        }

        /// <summary>
        /// Clear all Persons.
        /// </summary>
        public void ClearPersons()
        {
            _peopleCollection?.DeleteAll();
        }

        /// <summary>
        /// Add a new <see cref="Person"/> to the list of Persons.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        public void AddPerson(Person person)
        {
            updatePersonProperties(person);
            _peopleCollection?.Insert(person);
        }

        /// <summary>
        /// Update a <see cref="Person"/> object with the given one.
        /// </summary>
        /// <param name="person">New <see cref="Person"/> object</param>
        public void UpdatePerson(Person person)
        {
            if(person == null) { return; }
            updatePersonProperties(person);
            _peopleCollection?.Update(person);
        }

        /// <summary>
        /// Return the number of <see cref="Person"/> in the database
        /// </summary>
        /// <returns>Number of <see cref="Person"/> in the database</returns>
        public int GetPersonCount()
        {
            return _peopleCollection?.Count() ?? 0;
        }

        /// <summary>
        /// Get the number of <see cref="Person"/> with parsing errors/>
        /// </summary>
        /// <returns>number of <see cref="Person"/> with parsing errors</returns>
        public int GetParsingErrorCount()
        {
            List<Person> people = _peopleCollection?.Query().ToList();
            return people.Where(p => !string.IsNullOrEmpty(p.ParsingFailureMessage)).Count();
        }

        /// <summary>
        /// Get the number of available (but not obtained) BLSV <see cref="Reward"/>
        /// </summary>
        /// <returns>Number of available (but not obtained) BLSV <see cref="Reward"/></returns>
        public int GetAvailableBLSVRewardsCount()
        {
            List<Person> people = GetPersons();
            return people.Count(p => p.Rewards.HighestAvailableBLSVReward != null);
        }

        /// <summary>
        /// Get the number of available (but not obtained) TSV <see cref="Reward"/>
        /// </summary>
        /// <returns>Number of available (but not obtained) TSV <see cref="Reward"/></returns>
        public int GetAvailableTSVRewardsCount()
        {
            List<Person> people = GetPersons();
            return people.Count(p => p.Rewards.HighestAvailableTSVReward != null);
        }


        /// <summary>
        /// Update the following properties of the given Person:
        /// - <see cref="Person.ScoreBLSV"/>
        /// - <see cref="Person.ScoreTSV"/>
        /// - <see cref="Person.EffectiveBoardMemberYears"/>
        /// - <see cref="Person.EffectiveHeadOfDepartementYears"/>
        /// - <see cref="Person.EffectiveOtherFunctionsYears"/>
        /// - <see cref="Person.Rewards"/>
        /// </summary>
        /// <param name="person">Person to update</param>
        private void updatePersonProperties(Person person)
        {
            person.MembershipYears = (int)Math.Ceiling((CalculationDeadline - person.EntryDate).TotalDays / 365);
            if(person.MembershipYears < 0) { person.MembershipYears = 0; }  // The membership years should be 0 or greater. It would be negative if CalculationDeadline < EntryDate.

            // ***** Effective score time periods *****
            List<Function> functionsBoardMember = person.Functions.Where(f => f.Type == FunctionType.BOARD_MEMBER).ToList();
            List<Function> functionsHeadOfDepartement = person.Functions.Where(f => f.Type == FunctionType.HEAD_OF_DEPARTEMENT).ToList();
            List<Function> functionsOther = person.Functions.Where(f => f.Type == FunctionType.OTHER_FUNCTION).ToList();

            // Replace the function end date with the CalculationDeadline if the function is ongoing
            functionsBoardMember.ForEach(f => f.TimePeriod.End = (!f.IsFunctionOngoing ? f.TimePeriod.End : (CalculationDeadline > f.TimePeriod.Start ? CalculationDeadline : f.TimePeriod.Start)));
            functionsHeadOfDepartement.ForEach(f => f.TimePeriod.End = (!f.IsFunctionOngoing ? f.TimePeriod.End : (CalculationDeadline > f.TimePeriod.Start ? CalculationDeadline : f.TimePeriod.Start)));
            functionsOther.ForEach(f => f.TimePeriod.End = (!f.IsFunctionOngoing ? f.TimePeriod.End : (CalculationDeadline > f.TimePeriod.Start ? CalculationDeadline : f.TimePeriod.Start)));

            TimePeriodCollection boardMemberPeriods = new TimePeriodCollection(functionsBoardMember.Select(f => f.TimePeriod));
            TimePeriodCollection headOfDepartementPeriods = new TimePeriodCollection(functionsHeadOfDepartement.Select(f => f.TimePeriod));
            TimePeriodCollection otherPeriods = new TimePeriodCollection(functionsOther.Select(f => f.TimePeriod));

            // Consolidate the time periods for each function type
            TimePeriodCombiner<TimeRange> periodCombiner = new TimePeriodCombiner<TimeRange>();
            boardMemberPeriods = (TimePeriodCollection)periodCombiner.CombinePeriods(boardMemberPeriods);
            headOfDepartementPeriods = (TimePeriodCollection)periodCombiner.CombinePeriods(headOfDepartementPeriods);
            otherPeriods = (TimePeriodCollection)periodCombiner.CombinePeriods(otherPeriods);

            TimePeriodSubtractor<TimeRange> subtractor = new TimePeriodSubtractor<TimeRange>();
            // Limit all time ranges to the CalculationDeadline
            TimePeriodCollection afterCalculationDeadline = new TimePeriodCollection() { new TimeRange(CalculationDeadline, DateTime.Now) };
            boardMemberPeriods = (TimePeriodCollection)subtractor.SubtractPeriods(boardMemberPeriods, afterCalculationDeadline);
            headOfDepartementPeriods = (TimePeriodCollection)subtractor.SubtractPeriods(headOfDepartementPeriods, afterCalculationDeadline);
            otherPeriods = (TimePeriodCollection)subtractor.SubtractPeriods(otherPeriods, afterCalculationDeadline);

            headOfDepartementPeriods = (TimePeriodCollection)subtractor.SubtractPeriods(headOfDepartementPeriods, boardMemberPeriods);
            otherPeriods = (TimePeriodCollection)subtractor.SubtractPeriods(otherPeriods, boardMemberPeriods);

            otherPeriods = (TimePeriodCollection)subtractor.SubtractPeriods(otherPeriods, headOfDepartementPeriods);

            // ***** TSV score *****
            person.EffectiveBoardMemberYears = (int)Math.Ceiling((boardMemberPeriods?.TotalDuration.TotalDays ?? 0) / 365);
            person.EffectiveHeadOfDepartementYears = (int)Math.Ceiling((headOfDepartementPeriods?.TotalDuration.TotalDays ?? 0) / 365);
            person.EffectiveOtherFunctionsYears = (int)Math.Ceiling((otherPeriods?.TotalDuration.TotalDays ?? 0) / 365);

            person.ScoreTSV = person.MembershipYears * 1 +                      // One point per membership year
                                person.EffectiveBoardMemberYears * 3 +          // Three points per year as board member
                                person.EffectiveHeadOfDepartementYears * 2 +    // Two points per year as head of departement
                                person.EffectiveOtherFunctionsYears * 1;        // One point per year in any other function

            // ***** BLSV score *****
            person.ScoreBLSV = person.MembershipYears * 1;  // One point per year of membership

            // ***** Update Rewards *****
            person.Rewards.BLSV20.Available = (person.ScoreBLSV >= 20);
            person.Rewards.BLSV25.Available = (person.ScoreBLSV >= 25);
            person.Rewards.BLSV30.Available = (person.ScoreBLSV >= 30);
            person.Rewards.BLSV40.Available = (person.ScoreBLSV >= 40);
            person.Rewards.BLSV45.Available = (person.ScoreBLSV >= 45);
            person.Rewards.BLSV50.Available = (person.ScoreBLSV >= 50);
            person.Rewards.BLSV60.Available = (person.ScoreBLSV >= 60);
            person.Rewards.BLSV70.Available = (person.ScoreBLSV >= 70);
            person.Rewards.BLSV80.Available = (person.ScoreBLSV >= 80);
            person.Rewards.TSVSilver.Available = (person.ScoreTSV >= REWARD_TSVSILVER_POINTS);
            person.Rewards.TSVGold.Available = (person.ScoreTSV >= REWARD_TSVGOLD_POINTS);
            person.Rewards.TSVHonorary.Available = (person.ScoreTSV >= REWARD_TSVHONORARY_POINTS);
        }

    }
}
