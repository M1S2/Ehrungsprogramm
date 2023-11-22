using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ehrungsprogramm.Core.Models;

namespace Ehrungsprogramm.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to print various objects
    /// </summary>
    public interface IPrintService
    {
        /// <summary>
        /// Print details of a single person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that should be printed</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        Task<bool> PrintPerson(Person person, string pdfFilePath);

        /// <summary>
        /// Print an overview list of all people.
        /// </summary>
        /// <param name="people">List with all available <see cref="Person"/> objects. This list may be filtered.</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <param name="fullPeopleCount">Number of all (unfilteted) people.</param>
        /// <param name="filterText">Text used to filter the people list (empty string if not filtered)</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        Task<bool> PrintPersonList(List<Person> people, string pdfFilePath, int fullPeopleCount, string filterText = "");

        /// <summary>
        /// Print an overview of all TSV rewards.
        /// </summary>
        /// <param name="peopleTsvRewardAvailable">List with all available <see cref="Person"/> objects with available TSV rewards used to generate the rewards overview</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <param name="fullTsvRewardsCount">Number of all (unfiltered) TSV rewards.</param>
        /// <param name="filterTextTsv">String indicating, which filters were applied to the TSV reward list</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        Task<bool> PrintTsvRewards(List<Person> peopleTsvRewardAvailable, string pdfFilePath, int fullTsvRewardsCount, string filterTextTsv);

        /// <summary>
        /// Print an overview of all BLSV rewards.
        /// </summary>
        /// <param name="peopleBlsvRewardAvailable">List with all available <see cref="Person"/> objects with available BLSV rewards used to generate the rewards overview</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <param name="fullBlsvRewardsCount">Number of all (unfiltered) BLSV rewards.</param>
        /// <param name="filterTextBlsv">String indicating, which filters were applied to the BLSV reward list</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        Task<bool> PrintBlsvRewards(List<Person> peopleBlsvRewardAvailable, string pdfFilePath, int fullBlsvRewardsCount, string filterTextBlsv);
    }
}
