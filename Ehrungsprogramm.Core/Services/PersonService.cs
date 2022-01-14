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
            _database = new LiteDatabase(@"D:\Benutzer\V17\Desktop\Ehrungsprogramm_Persons.db");
            _peopleCollection = _database.GetCollection<Person>("people");

            /*List<Person> People = new List<Person>()
            {
                new Person() { FirstName = "Max", Name = "Mustermann", Score = 26 },
                new Person() { FirstName = "Eva", Name = "Musterfrau", Score = 54 },
                new Person() { FirstName = "Abraham", Name = "Lincoln", Score = 100 }
            };
            People.ForEach(p => AddPerson(p));*/
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
