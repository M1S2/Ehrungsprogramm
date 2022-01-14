using System;
using System.Collections.Generic;
using System.Text;
using Ehrungsprogramm.Core.Models;

namespace Ehrungsprogramm.Core.Contracts.Services
{
    public interface IPersonService
    {
        List<Person> GetPersons();
        void ClearPersons();
        void AddPerson(Person person);
        void UpdatePerson(Person person);
    }
}
