using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Core.Models;

namespace Ehrungsprogramm.ViewModels
{
    public class ManageDatabaseViewModel : ObservableObject
    {
        private ICommand _clearDatabaseCommand;
        public ICommand ClearDatabaseCommand => _clearDatabaseCommand ?? (_clearDatabaseCommand = new RelayCommand(() => _personService.ClearPersons()));

        private ICommand _addPersonTestCommand;
        public ICommand AddPersonTestCommand => _addPersonTestCommand ?? (_addPersonTestCommand = new RelayCommand(() => _personService.AddPerson(new Person() { FirstName = "Test First Name", Name = "Test Name", EntryDate = DateTime.Now - TimeSpan.FromDays(new Random().Next(0, 365 * 50)), ScoreTSV = new Random().Next(0, 100) })));

        private IPersonService _personService;

        public ManageDatabaseViewModel(IPersonService personService)
        {
            _personService = personService;
        }
    }
}
