﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ehrungsprogramm.Core.Models;

namespace Ehrungsprogramm.Core.Contracts.Services
{
    /// <summary>
    /// Delegate void for progress changes
    /// </summary>
    /// <param name="filepath">filepath to the file to import</param>
    /// <param name="progress">Progress 0 .. 100 </param>
    public delegate void ProgressDelegate(string filepath, float progress);


    /// <summary>
    /// Interface for a service used to get and store a list of Person objects
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Import a list of Persons to an internal database.
        /// </summary>
        /// <param name="filepath">filepath to the file to import</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        Task<bool> ImportFromFile(string filepath, CancellationToken cancellationToken);

        /// <summary>
        /// Event that is raised when the import progress changes
        /// </summary>
        event ProgressDelegate OnImportFromFileProgress;

        /// <summary>
        /// Event that is raised when the import from the file is finished.
        /// </summary>
        event EventHandler OnImportFromFileFinished;

        /// <summary>
        /// End date that is used while calculating the membership years and the years for each function
        /// </summary>
        DateTime CalculationDeadline { get; set; }

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
