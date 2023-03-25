using System;
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
        /// Path of the last imported file
        /// </summary>
        string LastImportFilePath { get; set; }

        /// <summary>
        /// Return all available Persons.
        /// </summary>
        /// <returns>List of <see cref="Person"/> objects</returns>
        List<Person> GetPersons();

        /// <summary>
        /// Clear all Persons.
        /// </summary>
        void ClearPersons();

        /// <summary>
        /// Add a new <see cref="Person"/> to the list of Persons.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        void AddPerson(Person person);

        /// <summary>
        /// Return the number of <see cref="Person"/> in the database
        /// </summary>
        /// <returns>Number of <see cref="Person"/> in the database</returns>
        int PersonCount { get; }

        /// <summary>
        /// Get the number of <see cref="Person"/> with parsing errors/>
        /// </summary>
        /// <returns>number of <see cref="Person"/> with parsing errors</returns>
        int ParsingErrorCount { get; }

        /// <summary>
        /// Number of functions of type <see cref="FunctionType.UNKNOWN"/> within all <see cref="Person"/>/>
        /// </summary>
        int UnknownFunctionsCount { get; }

        /// <summary>
        /// Get the number of available (but not obtained) BLSV <see cref="Reward"/>
        /// </summary>
        /// <returns>Number of available (but not obtained) BLSV <see cref="Reward"/></returns>
        int AvailableBLSVRewardsCount { get; }

        /// <summary>
        /// Get the number of available (but not obtained) TSV <see cref="Reward"/>
        /// </summary>
        /// <returns>Number of available (but not obtained) TSV <see cref="Reward"/></returns>
        int AvailableTSVRewardsCount { get; }
    }
}
