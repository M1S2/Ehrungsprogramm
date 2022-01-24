using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Core.Models;
using LiteDB;

namespace Ehrungsprogramm.Core.Services
{
    public class PersonService : IPersonService
    {
        private LiteDatabase _database;
        private ILiteCollection<Person> _peopleCollection;

        public PersonService()
        {
            // TODO Replace fixed filepath for batabase by some kind of property
            _database = new LiteDatabase(@"S:\IT\Ehrungsprogramm\Ehrungsprogramm_Persons.db");
            _peopleCollection = _database.GetCollection<Person>("people");
        }

        public void ImportFromFile(string filepath)
        {
            List<Person> importedPeople = CsvFileParserProWinner.Parse(filepath);
            ClearPersons();
            _peopleCollection?.InsertBulk(importedPeople);
        }

        public List<Person> GetPersons()
        {
            List<Person> people = _peopleCollection?.Query().ToList();
            people.ForEach(p => updatePersonProperties(p));
            return people;
        }

        public void ClearPersons()
        {
            _peopleCollection?.DeleteAll();
        }

        public void AddPerson(Person person)
        {
            updatePersonProperties(person);
            _peopleCollection?.Insert(person);
        }

        public void UpdatePerson(Person person)
        {
            if(person == null) { return; }
            updatePersonProperties(person);
            _peopleCollection?.Update(person);
        }


        private void updatePersonProperties(Person person)
        {
            // ***** BLSV score *****
            person.ScoreBLSV = person.MembershipYears * 1;  // One point per year of membership

            // ***** Effective score time periods *****
            List<Function> functionsBoardMember = person.Functions.Where(f => f.Type == FunctionType.BOARD_MEMBER).ToList();
            List<Function> functionsHeadOfDepartement = person.Functions.Where(f => f.Type == FunctionType.HEAD_OF_DEPARTEMENT).ToList();
            List<Function> functionsOther = person.Functions.Where(f => f.Type == FunctionType.OTHER_FUNCTION).ToList();

            person.Functions.ForEach(f => f.EffectiveScoreTimePeriods.Clear());
            DateTimeRange additionalRange = null;

            // Loop over all board member function entries and compare them against the other functions
            foreach (Function funcBM in functionsBoardMember)
            {
                funcBM.EffectiveScoreTimePeriods.Add(funcBM.TimePeriod);

                foreach (Function funcHead in functionsHeadOfDepartement)
                {
                    funcHead.EffectiveScoreTimePeriods.Add(funcHead.TimePeriod.Subtract(funcBM.TimePeriod, out additionalRange));
                    if (additionalRange != null) { funcHead.EffectiveScoreTimePeriods.Add(additionalRange); }
                }
                foreach (Function funcOther in functionsOther)
                {
                    funcOther.EffectiveScoreTimePeriods.Add(funcOther.TimePeriod.Subtract(funcBM.TimePeriod, out additionalRange));
                    if (additionalRange != null) { funcOther.EffectiveScoreTimePeriods.Add(additionalRange); }
                }
            }

            foreach (Function funcOther in functionsOther)
            {
                List<DateTimeRange> newFuncOtherEffectivePeriods = new List<DateTimeRange>();
                foreach (DateTimeRange funcOtherEffectivePeriod in funcOther.EffectiveScoreTimePeriods)
                {
                    foreach (Function funcHead in functionsHeadOfDepartement)
                    {
                        foreach (DateTimeRange funcHeadEffectivePeriod in funcHead.EffectiveScoreTimePeriods)
                        {
                            newFuncOtherEffectivePeriods.Add(funcOtherEffectivePeriod?.Subtract(funcHeadEffectivePeriod, out additionalRange));
                            if (additionalRange != null) { newFuncOtherEffectivePeriods.Add(additionalRange); }
                        }
                    }
                }
                funcOther.EffectiveScoreTimePeriods = newFuncOtherEffectivePeriods;
            }

            // ***** TSV score *****
            person.ScoreTSV = 0;
            foreach (Function func in person.Functions)
            {
                int points = 0;
                switch(func.Type)
                {
                    case FunctionType.BOARD_MEMBER: points = 3; break;
                    case FunctionType.HEAD_OF_DEPARTEMENT: points = 2; break;
                    case FunctionType.OTHER_FUNCTION: points = 1; break;
                    default: break;
                }
                points *= func.EffectiveScoreYears;
                person.ScoreTSV += points;
            }
            person.ScoreTSV += person.MembershipYears * 1;      // One point per membership year
        }

    }
}
