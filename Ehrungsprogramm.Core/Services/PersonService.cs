using System.Collections.Generic;
using System.IO;
using System.Text;

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
            _peopleCollection.InsertBulk(importedPeople);
        }

        public List<Person> GetPersons() => _peopleCollection.Query().ToList();
        public void ClearPersons() => _peopleCollection.DeleteAll();
        public void AddPerson(Person person) => _peopleCollection.Insert(person);
        public void UpdatePerson(Person person)
        {
            if(person == null) { return; }
            _peopleCollection.Update(person);
        }
    }
}
