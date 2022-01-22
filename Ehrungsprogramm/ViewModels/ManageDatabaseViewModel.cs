using System;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
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

        private ICommand _importDataFromFileCommand;
        public ICommand ImportDataFromFileCommand => _importDataFromFileCommand ?? (_importDataFromFileCommand = new RelayCommand(() => ImportFromFile()));

        private IPersonService _personService;

        public ManageDatabaseViewModel(IPersonService personService)
        {
            _personService = personService;
        }

        public void ImportFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "CSV File|*.csv|TXT File|*.txt",
            };

            if (openFileDialog.ShowDialog().Value)
            {
                _personService?.ImportFromFile(openFileDialog.FileName);
            }
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
                Functions = new List<Function>()
                {
                    new Function()
                    {
                        Type = FunctionType.OTHER_FUNCTION,
                        TimePeriod = new DateTimeRange(new DateTime(2010, 01, 01), new DateTime(2015, 01, 01)),
                        Description = "Schwimmen-FKT Helfer"
                    },
                    new Function()
                    {
                        Type = FunctionType.BOARD_MEMBER,
                        TimePeriod = new DateTimeRange(new DateTime(2015, 01, 01), new DateTime(2019, 01, 01)),
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
                Functions = new List<Function>()
                {
                    new Function()
                    {
                        Type = FunctionType.BOARD_MEMBER,
                        TimePeriod = new DateTimeRange(new DateTime(8, 01, 01), new DateTime(16, 01, 01)),
                        Description = "BM"
                    },
                    new Function()
                    {
                        Type = FunctionType.HEAD_OF_DEPARTEMENT,
                        TimePeriod = new DateTimeRange(new DateTime(6, 01, 01), new DateTime(12, 01, 01)),
                        Description = "HEAD 1"
                    },
                    new Function()
                    {
                        Type = FunctionType.HEAD_OF_DEPARTEMENT,
                        TimePeriod = new DateTimeRange(new DateTime(14, 01, 01), new DateTime(20, 01, 01)),
                        Description = "HEAD 2"
                    },
                    new Function()
                    {
                        Type = FunctionType.OTHER_FUNCTION,
                        TimePeriod = new DateTimeRange(new DateTime(2, 01, 01), new DateTime(10, 01, 01)),
                        Description = "FKT 1"
                    },
                    new Function()
                    {
                        Type = FunctionType.OTHER_FUNCTION,
                        TimePeriod = new DateTimeRange(new DateTime(4, 01, 01), new DateTime(23, 01, 01)),
                        Description = "FKT 2"
                    }
                }
            });



            for (int i = 0; i < 100; i++)
            {
                _personService?.AddPerson(new Person()
                {
                    FirstName = "Eva" + i.ToString(),
                    Name = "Musterfrau" + i.ToString(),
                    BirthDate = new DateTime(1950, 02, 03),
                    EntryDate = new DateTime(1980, 01, 01),
                    Functions = new List<Function>()
                {
                    new Function()
                    {
                        Type = FunctionType.OTHER_FUNCTION,
                        TimePeriod = new DateTimeRange(new DateTime(1090, 01, 01), new DateTime(2010, 01, 01)),
                        Description = "Turnen-FKT ÜL"
                    }
                }
                });
            }
        }
    }
}
