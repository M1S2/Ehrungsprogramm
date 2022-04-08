using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Ehrungsprogramm.Core.Models;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Contracts.Services;
using Ehrungsprogramm.Contracts.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace Ehrungsprogramm.ViewModels
{
    public class PersonsViewModel : ObservableObject, INavigationAware
    {
        private List<Person> _people;
        public List<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        public Func<object, string, bool> PersonFilter
        {
            get
            {
                return (item, text) =>
                {
                    Person person = item as Person;
                    return person.Name.ToLower().Contains(text.ToLower()) 
                            || person.FirstName.ToLower().Contains(text.ToLower())
                            || (person.FirstName.ToLower() + " " + person.Name.ToLower()).Contains(text.ToLower())
                            || (person.Name.ToLower() + " " + person.FirstName.ToLower()).Contains(text.ToLower())
                            || (person.FirstName.ToLower() + ", " + person.Name.ToLower()).Contains(text.ToLower())
                            || (person.Name.ToLower() + ", " + person.FirstName.ToLower()).Contains(text.ToLower())
                            || person.ScoreBLSV.ToString().Contains(text)
                            || person.ScoreTSV.ToString().Contains(text)
                            || person.EntryDate.ToString().Contains(text);
                };
            }
        }

        private ICommand _personDetailCommand;
        public ICommand PersonDetailCommand => _personDetailCommand ?? (_personDetailCommand = new RelayCommand<Person>((person) => _navigationService.NavigateTo(typeof(PersonDetailViewModel).FullName, person)));

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
                await _printService?.PrintPersonList(People);
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
        },
        () => !_isPrinting));


        private IPersonService _personService;
        private IPrintService _printService;
        private INavigationService _navigationService;
        private IDialogCoordinator _dialogCoordinator;

        public PersonsViewModel(IPersonService personService, IPrintService printService, INavigationService navigationService, IDialogCoordinator dialogCoordinator)
        {
            _personService = personService;
            _printService = printService;
            _navigationService = navigationService;
            _dialogCoordinator = dialogCoordinator;
            People = new List<Person>();
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo(object parameter)
        {
            People.Clear();
            List<Person> servicePeople = _personService?.GetPersons();
            servicePeople?.ForEach(p => People.Add(p));
            OnPropertyChanged(nameof(People));
        }
    }
}
