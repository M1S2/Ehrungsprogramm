using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
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
        private const int REWARD_TSVSILVER_POINTS = 25;
        private const int REWARD_TSVGOLD_POINTS = 50;
        private const int REWARD_TSVHONORARY_POINTS = 75;

        private LiteDatabase _database;                         // Handle to a database holding the collection of Persons
        private ILiteCollection<Person> _peopleCollection;      // Collection of Persons

        /// <summary>
        /// Constructor that opens a database from file and gets all people from the database.
        /// </summary>
        public PersonService()
        {
            // TODO Replace fixed filepath for batabase by some kind of property
            _database = new LiteDatabase(@"S:\IT\Ehrungsprogramm\Ehrungsprogramm_Persons.db");
            _peopleCollection = _database.GetCollection<Person>("people");
        }

        /// <summary>
        /// Import a list of Personsto an internal database.
        /// </summary>
        /// <param name="filepath">filepath of the database</param>
        public void ImportFromFile(string filepath)
        {
            List<Person> importedPeople = CsvFileParserProWinner.Parse(filepath);
            ClearPersons();
            _peopleCollection?.InsertBulk(importedPeople);
        }

        /// <summary>
        /// Return all available Persons.
        /// </summary>
        /// <returns>List of Person objects</returns>
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
        /// Add a new Person to the list of Persons.
        /// </summary>
        /// <param name="person">Person to add</param>
        public void AddPerson(Person person)
        {
            updatePersonProperties(person);
            _peopleCollection?.Insert(person);
        }

        /// <summary>
        /// Update a Person object with the given one.
        /// </summary>
        /// <param name="person">New Person object</param>
        public void UpdatePerson(Person person)
        {
            if(person == null) { return; }
            updatePersonProperties(person);
            _peopleCollection?.Update(person);
        }


        /// <summary>
        /// Update the following properties of the given Person:
        /// - <see cref="Person.ScoreBLSV"/>
        /// - <see cref="Person.ScoreTSV"/>
        /// - <see cref="Person.EffectiveBoardMemberYears"/>
        /// - <see cref="Person.EffectiveHeadOfDepartementYears"/>
        /// - <see cref="Person.EffectiveOtherFunctionsYears"/>
        /// </summary>
        /// <param name="person">Person to update</param>
        private void updatePersonProperties(Person person)
        {
            // ***** Effective score time periods *****
            List<Function> functionsBoardMember = person.Functions.Where(f => f.Type == FunctionType.BOARD_MEMBER).ToList();
            List<Function> functionsHeadOfDepartement = person.Functions.Where(f => f.Type == FunctionType.HEAD_OF_DEPARTEMENT).ToList();
            List<Function> functionsOther = person.Functions.Where(f => f.Type == FunctionType.OTHER_FUNCTION).ToList();

            TimePeriodCollection boardMemberPeriods = new TimePeriodCollection(functionsBoardMember.Select(f => f.TimePeriod));
            TimePeriodCollection headOfDepartementPeriods = new TimePeriodCollection(functionsHeadOfDepartement.Select(f => f.TimePeriod));
            TimePeriodCollection otherPeriods = new TimePeriodCollection(functionsOther.Select(f => f.TimePeriod));

            // Consolidate the time periods for each function type
            TimePeriodCombiner<TimeRange> periodCombiner = new TimePeriodCombiner<TimeRange>();
            boardMemberPeriods = (TimePeriodCollection)periodCombiner.CombinePeriods(boardMemberPeriods);
            headOfDepartementPeriods = (TimePeriodCollection)periodCombiner.CombinePeriods(headOfDepartementPeriods);
            otherPeriods = (TimePeriodCollection)periodCombiner.CombinePeriods(otherPeriods);

            TimePeriodSubtractor<TimeRange> subtractor = new TimePeriodSubtractor<TimeRange>();
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
