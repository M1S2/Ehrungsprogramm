using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ehrungsprogramm.Core.Models;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Contracts.Services;
using Ehrungsprogramm.Contracts.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace Ehrungsprogramm.ViewModels
{
    public class PersonDetailViewModel : ObservableObject, INavigationAware
    {
        private Person _person;
        /// <summary>
        /// Person object that is used to show the detail page.
        /// </summary>
        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        private bool _isPrinting;
        /// <summary>
        /// True, if printing person detail overview. False if not currently printing.
        /// </summary>
        public bool IsPrinting
        {
            get => _isPrinting;
            set { SetProperty(ref _isPrinting, value); ((RelayCommand)PrintCommand).NotifyCanExecuteChanged(); }
        }

        private ICommand _printCommand;
        /// <summary>
        /// Command used to print a person detail overview.
        /// </summary>
        public ICommand PrintCommand => _printCommand ?? (_printCommand = new RelayCommand(async () =>
        {
            try
            {
                IsPrinting = true;
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog() { FileName = string.Format(Properties.Resources.DefaultFileNamePersonDetail, Person?.Name + "_" + Person?.FirstName), Filter = Properties.Resources.FileFilterPDF };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    await _printService?.PrintPerson(Person, saveFileDialog.FileName);
                    await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.PrintString, Properties.Resources.PrintString + " " + Properties.Resources.SuccessfulString.ToLower());
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.PrintString, Properties.Resources.ErrorString + ": " + ex.Message);
                }
                catch (Exception)
                {
                    /* Error couldn't be displayed. No action needed here. */
                }
            }
            finally
            {
                IsPrinting = false;
            }
        }));

        private IPrintService _printService;
        private IDialogCoordinator _dialogCoordinator;

        public PersonDetailViewModel(IPrintService printService, IDialogCoordinator dialogCoordinator)
        {
            _printService = printService;
            _dialogCoordinator = dialogCoordinator;
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo(object parameter)
        {
            Person = parameter as Person;
        }
    }
}
