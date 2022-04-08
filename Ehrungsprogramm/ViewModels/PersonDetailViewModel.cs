using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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
        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        private bool _isPrinting;
        public bool IsPrinting
        {
            get => _isPrinting;
            set { SetProperty(ref _isPrinting, value); ((RelayCommand)PrintCommand).NotifyCanExecuteChanged(); }
        }

        private ICommand _printCommand;
        public ICommand PrintCommand => _printCommand ?? (_printCommand = new RelayCommand(async () =>
        {
            try
            {
                IsPrinting = true;
                await _printService?.PrintPerson(Person);
                await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.PrintString, Properties.Resources.PrintString + " " + Properties.Resources.SuccessfulString.ToLower());
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.PrintString, Properties.Resources.ErrorString + ": " + ex.Message);
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
