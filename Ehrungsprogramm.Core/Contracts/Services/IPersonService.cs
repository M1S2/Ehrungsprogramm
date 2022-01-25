using System;
using System.Collections.Generic;
using System.Text;
using Ehrungsprogramm.Core.Models;

namespace Ehrungsprogramm.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to get and store a list of Person objects
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Import a list of Personsto an internal database.
        /// </summary>
        /// <param name="filepath">filepath of the database</param>
        void ImportFromFile(string filepath);

        /// <summary>
        /// Return all available Persons.
        /// </summary>
        /// <returns>List of Person objects</returns>
        List<Person> GetPersons();

        /// <summary>
        /// Clear all Persons.
        /// </summary>
        void ClearPersons();

        /// <summary>
        /// Add a new Person to the list of Persons.
        /// </summary>
        /// <param name="person">Person to add</param>
        void AddPerson(Person person);

        /// <summary>
        /// Update a Person object with the given one.
        /// </summary>
        /// <param name="person">New Person object</param>
        void UpdatePerson(Person person);
    }
}
