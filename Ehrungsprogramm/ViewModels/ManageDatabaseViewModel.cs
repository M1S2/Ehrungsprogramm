using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Core.Models;
using Itenso.TimePeriod;
using MahApps.Metro.Controls.Dialogs;
using Ehrungsprogramm.Contracts.Services;

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
                RefreshStatistics();
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


        private ICommand _clearDatabaseCommand;
        public ICommand ClearDatabaseCommand => _clearDatabaseCommand ?? (_clearDatabaseCommand = new RelayCommand(async() => await ClearDatabase()));

        private ICommand _importDataFromFileCommand;
        public ICommand ImportDataFromFileCommand => _importDataFromFileCommand ?? (_importDataFromFileCommand = new RelayCommand(async() => await ImportFromFile()));

        private ICommand _reloadDataFileCommand;
        public ICommand ReloadDataFileCommand => _reloadDataFileCommand ?? (_reloadDataFileCommand = new RelayCommand(async () => await ImportFromFile(LastImportFilePath)));

        private ICommand _showParsingFailurePersonsCommand;
        public ICommand ShowParsingFailurePersonsCommand => _showParsingFailurePersonsCommand ?? (_showParsingFailurePersonsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PersonsViewModel).FullName, PersonsViewModel.FILTER_FLAG_WARN)));

        private ICommand _setCalculationDeadlineTodayCommand;
        public ICommand SetCalculationDeadlineTodayCommand => _setCalculationDeadlineTodayCommand ?? (_setCalculationDeadlineTodayCommand = new RelayCommand(() => CalculationDeadline = DateTime.Now));

        private IPersonService _personService;
        private IDialogCoordinator _dialogCoordinator;
        private INavigationService _navigationService;
        private ProgressDialogController _progressController;

        public ManageDatabaseViewModel(IPersonService personService, IDialogCoordinator dialogCoordinator, INavigationService navigationService)
        {
            _personService = personService;
            _dialogCoordinator = dialogCoordinator;
            _navigationService = navigationService;

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
        /// Allow to drop files to the ManageDatabasePage and load the first dropped file (if it's .csv or .txt)
        /// </summary>
        public async void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> fileNames = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
                string firstFileName = fileNames.FirstOrDefault();
                if (System.IO.Path.GetExtension(firstFileName).ToLower() == ".csv" || System.IO.Path.GetExtension(firstFileName).ToLower() == ".txt")
                {
                    await ImportFromFile(firstFileName);
                }
            }
        }

        /// <summary>
        /// Import a list of Persons from a .csv or .txt file.
        /// </summary>
        /// <param name="filePath">If the path isn't empty, the given file is loaded; otherwise show a dialog to select a file</param>
        public async Task ImportFromFile(string filePath = "")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "CSV File|*.csv|TXT File|*.txt",
            };

            if (!string.IsNullOrEmpty(filePath) || openFileDialog.ShowDialog().Value)    // if filePath isn't empty, the dialog isn't opened
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                _progressController = await _dialogCoordinator.ShowProgressAsync(this, Properties.Resources.ImportDataFromFileString + "...", "", true);
                _progressController.Canceled += (sender, e) => cancellationTokenSource.Cancel();
                try
                {
                    await _personService?.ImportFromFile(!string.IsNullOrEmpty(filePath) ? filePath : openFileDialog.FileName, cancellationTokenSource.Token);
                    RefreshStatistics();
                }
                catch (Exception ex)
                {
                    await _progressController.CloseAsync();
                    await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.ErrorString, ex.Message);
                }
            }
        }

        /// <summary>
        /// Clear all people from the database
        /// </summary>
        private async Task ClearDatabase()
        {
            MessageDialogResult dialogResult = await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.ClearDatabaseString, Properties.Resources.ClearDatabaseConfirmationString, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = Properties.Resources.ClearDatabaseString, NegativeButtonText = Properties.Resources.CancelString });
            if (dialogResult == MessageDialogResult.Affirmative)
            {
                _personService?.ClearPersons();
                RefreshStatistics();
            }
        }

        /// <summary>
        /// Refresh all database statistic indicators by raising a property changed event
        /// </summary>
        private void RefreshStatistics()
        {
            OnPropertyChanged(nameof(PersonCount));
            OnPropertyChanged(nameof(ParsingErrorCount));
            OnPropertyChanged(nameof(AvailableBLSVRewardsCount));
            OnPropertyChanged(nameof(AvailableTSVRewardsCount));
            OnPropertyChanged(nameof(LastImportFilePath));
        }
    }
}
