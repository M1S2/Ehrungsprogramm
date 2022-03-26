using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        /// <summary>
        /// End date that is used while calculating the membership years and the years for each function
        /// This just sets or gets the property in the person service if available
        /// </summary>
        public DateTime CalculationDeadline
        {
            get => _personService?.CalculationDeadline ?? DateTime.Now;
            set 
            {
                if(_personService == null) { return; }
                _personService.CalculationDeadline = value; 
                OnPropertyChanged(nameof(CalculationDeadline)); 
            }
        }

        /// <summary>
        /// Number of Persons in the database
        /// </summary>
        public int PersonCount => _personService?.GetPersonCount() ?? 0;

        /// <summary>
        /// Number of <see cref="Person"/> with parsing errors/>
        /// </summary>
        public int ParsingErrorCount => _personService?.GetParsingErrorCount() ?? 0;

        /// <summary>
        /// Number of available (but not obtained) BLSV <see cref="Reward"/>
        /// </summary>
        public int AvailableBLSVRewardsCount => _personService?.GetAvailableBLSVRewardsCount() ?? 0;

        /// <summary>
        /// Number of available (but not obtained) TSV <see cref="Reward"/>
        /// </summary>
        public int AvailableTSVRewardsCount => _personService?.GetAvailableTSVRewardsCount() ?? 0;

        /// <summary>
        /// Path of the last imported file
        /// </summary>
        public string LastImportFilePath => _personService?.LastImportFilePath ?? "";


        // TODO Add confirmation dialog for ClearDatabase command
        private ICommand _clearDatabaseCommand;
        public ICommand ClearDatabaseCommand => _clearDatabaseCommand ?? (_clearDatabaseCommand = new RelayCommand(() => { _personService?.ClearPersons(); RefreshStatistics(); }));

        private ICommand _generateTestDataCommand;
        public ICommand GenerateTestDataCommand => _generateTestDataCommand ?? (_generateTestDataCommand = new RelayCommand(() => GenerateTestData()));

        private ICommand _importDataFromFileCommand;
        public ICommand ImportDataFromFileCommand => _importDataFromFileCommand ?? (_importDataFromFileCommand = new RelayCommand(async() => await ImportFromFile()));


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

        /// <summary>
        /// Import a list of Persons from a .csv or .txt file.
        /// </summary>
        public async Task ImportFromFile()
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
                try
                {
                    await _personService?.ImportFromFile(openFileDialog.FileName, cancellationTokenSource.Token);
                    RefreshStatistics();
                }
                catch (Exception ex) 
                {
                    await _progressController.CloseAsync();
                    await _dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
                }
            }
        }

        private void RefreshStatistics()
        {
            OnPropertyChanged(nameof(PersonCount));
            OnPropertyChanged(nameof(ParsingErrorCount));
            OnPropertyChanged(nameof(AvailableBLSVRewardsCount));
            OnPropertyChanged(nameof(AvailableTSVRewardsCount));
            OnPropertyChanged(nameof(LastImportFilePath));
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
                },
                ParsingFailureMessage = "Test Parsing Error Message"
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

            RefreshStatistics();
        }
    }
}
