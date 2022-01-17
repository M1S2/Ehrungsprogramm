using System;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Core.Models;

namespace Ehrungsprogramm.ViewModels
{
    public class ManageDatabaseViewModel : ObservableObject
    {
        // TODO Add confirmation dialog for ClearDatabase command
        private ICommand _clearDatabaseCommand;
        public ICommand ClearDatabaseCommand => _clearDatabaseCommand ?? (_clearDatabaseCommand = new RelayCommand(() => _personService?.ClearPersons()));

        private ICommand _generateTestDataCommand;
        public ICommand GenerateTestDataCommand => _generateTestDataCommand ?? (_generateTestDataCommand = new RelayCommand(() => GenerateTestData()));

        // TODO Replace fixed filepath for file import by some kind of property
        private ICommand _importDataFromFileCommand;
        public ICommand ImportDataFromFileCommand => _importDataFromFileCommand ?? (_importDataFromFileCommand = new RelayCommand(() => _personService?.ImportFromFile(@"S:\IT\Ehrungsprogramm\Listen 2019\TestDaten.csv")));

        private IPersonService _personService;

        public ManageDatabaseViewModel(IPersonService personService)
        {
            _personService = personService;
        }



        private void GenerateTestData()
        {
            _personService?.ClearPersons();

            _personService?.AddPerson(new Person()
            {
                FirstName = "Max",
                Name = "Mustermann",
                BirthDate = new DateTime(1990, 01, 01),
                EntryDate = new DateTime(2010, 01, 01),
                ScoreTSV = 30,
                Functions = new List<Function>()
                {
                    new Function()
                    {
                        Type = FunctionType.OTHER_FUNCTION,
                        StartDate = new DateTime(2010, 01, 01),
                        EndDate = new DateTime(2015, 01, 01),
                        Description = "Schwimmen-FKT Helfer"
                    },
                    new Function()
                    {
                        Type = FunctionType.BOARD_MEMBER,
                        StartDate = new DateTime(2015, 01, 01),
                        EndDate = new DateTime(2019, 01, 01),
                        Description = "1. Vorstand"
                    }
                }
            });

            _personService?.AddPerson(new Person()
            {
                FirstName = "Eva",
                Name = "Musterfrau",
                BirthDate = new DateTime(1950, 02, 03),
                EntryDate = new DateTime(1980, 01, 01),
                ScoreTSV = 60,
                Functions = new List<Function>()
                {
                    new Function()
                    {
                        Type = FunctionType.OTHER_FUNCTION,
                        StartDate = new DateTime(1090, 01, 01),
                        EndDate = new DateTime(2010, 01, 01),
                        Description = "Turnen-FKT ÜL"
                    }
                }
            });
        }
    }
}
