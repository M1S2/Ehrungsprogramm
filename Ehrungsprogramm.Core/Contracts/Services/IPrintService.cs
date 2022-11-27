﻿using System;
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
        /// Print an overview of all rewards.
        /// </summary>
        /// <param name="people">List with all available <see cref="Person"/> objects used to generate the rewards overview</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        Task<bool> PrintRewards(List<Person> people, string pdfFilePath);
    }
}
