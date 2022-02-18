using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Core.Models;
using Itenso.TimePeriod;
using MahApps.Metro.Controls.Dialogs;

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
        private IDialogCoordinator _dialogCoordinator;
        private ProgressDialogController _progressController;

        public ManageDatabaseViewModel(IPersonService personService, IDialogCoordinator dialogCoordinator)
        {
            _personService = personService;
            _dialogCoordinator = dialogCoordinator;
            
            _personService.OnImportFromFileProgress += (filepath, progress) =>
            {
                _progressController?.SetProgress(progress / 100);
                _progressController?.SetMessage(filepath + Environment.NewLine + (progress / 100).ToString("P0"));   // Format to percentage with 0 decimal digits
            };

            _personService.OnImportFromFileFinished += (sender, e) => 
            {
                try
                {
                    _progressController?.CloseAsync();
                }
                catch (Exception) { /* Nothing to do here. Seems already to be closed.*/}
            };
        }

        public async void ImportFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "CSV File|*.csv|TXT File|*.txt",
            };

            if (openFileDialog.ShowDialog().Value)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                _progressController = await _dialogCoordinator.ShowProgressAsync(this, Properties.Resources.ImportDataFromFileString + "...", "", true);
                _progressController.Canceled += (sender, e) => cancellationTokenSource.Cancel();
                _personService?.ImportFromFile(openFileDialog.FileName, cancellationTokenSource.Token);
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
                        TimePeriod = new TimeRange(new DateTime(2010, 01, 01), new DateTime(2015, 01, 01)),
                        Description = "Schwimmen-FKT Helfer"
                    },
                    new Function()
                    {
                        Type = FunctionType.BOARD_MEMBER,
                        TimePeriod = new TimeRange(new DateTime(2015, 01, 01), new DateTime(2019, 01, 01)),
                        Description = "1. Vorstand"
                    }
                }
            });

            Person tmpPerson = new Person()
            {
                FirstName = "Eva",
                Name = "Musterfrau",
                BirthDate = new DateTime(1950, 02, 03),
                EntryDate = new DateTime(1960, 01, 01),
                Functions = new List<Function>()
                {
                    new Function()
                    {
                        Type = FunctionType.BOARD_MEMBER,
                        TimePeriod = new TimeRange(new DateTime(8, 01, 01), new DateTime(16, 01, 01)),
                        Description = "BM"
                    },
                    new Function()
                    {
                        Type = FunctionType.HEAD_OF_DEPARTEMENT,
                        TimePeriod = new TimeRange(new DateTime(6, 01, 01), new DateTime(12, 01, 01)),
                        Description = "HEAD 1"
                    },
                    new Function()
                    {
                        Type = FunctionType.HEAD_OF_DEPARTEMENT,
                        TimePeriod = new TimeRange(new DateTime(14, 01, 01), new DateTime(20, 01, 01)),
                        Description = "HEAD 2"
                    },
                    new Function()
                    {
                        Type = FunctionType.OTHER_FUNCTION,
                        TimePeriod = new TimeRange(new DateTime(2, 01, 01), new DateTime(10, 01, 01)),
                        Description = "FKT 1"
                    },
                    new Function()
                    {
                        Type = FunctionType.OTHER_FUNCTION,
                        TimePeriod = new TimeRange(new DateTime(4, 01, 01), new DateTime(23, 01, 01)),
                        Description = "FKT 2"
                    }
                }
            };
            tmpPerson.Rewards.BLSV40.Obtained = true;
            tmpPerson.Rewards.BLSV40.ObtainedDate = new DateTime(2000, 01, 01);
            tmpPerson.Rewards.TSVSilver.Obtained = true;
            tmpPerson.Rewards.TSVSilver.ObtainedDate = new DateTime(2000, 01, 01);
            _personService?.AddPerson(tmpPerson);



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
                        TimePeriod = new TimeRange(new DateTime(1990, 01, 01), new DateTime(2010, 01, 01)),
                        Description = "Turnen-FKT ÜL"
                    }
                }
                });
            }
        }
    }
}
