﻿using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Ehrungsprogramm.Core.Models;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Contracts.Services;
using Ehrungsprogramm.Contracts.ViewModels;

namespace Ehrungsprogramm.ViewModels
{
    public class RewardsViewModel : ObservableObject, INavigationAware
    {
        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }


        private ICommand _personDetailCommand;
        public ICommand PersonDetailCommand => _personDetailCommand ?? (_personDetailCommand = new RelayCommand<Person>((person) => _navigationService.NavigateTo(typeof(PersonDetailViewModel).FullName, person)));


        private IPersonService _personService;
        private INavigationService _navigationService;

        public RewardsViewModel(IPersonService personService, INavigationService navigationService)
        {
            _personService = personService;
            _navigationService = navigationService;
            People = new ObservableCollection<Person>();
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo(object parameter)
        {
            People.Clear();
            List<Person> servicePeople = _personService?.GetPersons();
            servicePeople?.ForEach(p => People.Add(p));
        }
    }
}