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
        Task<bool> PrintPerson(Person person);

        /// <summary>
        /// Print an overview list of all people.
        /// </summary>
        /// <param name="people">List with all available <see cref="Person"/> objects</param>
        Task<bool> PrintPersonList(List<Person> people);

        /// <summary>
        /// Print an overview of all rewards.
        /// </summary>
        /// <param name="people">List with all available <see cref="Person"/> objects used to generate the rewards overview</param>
        Task<bool> PrintRewards(List<Person> people);
    }
}
