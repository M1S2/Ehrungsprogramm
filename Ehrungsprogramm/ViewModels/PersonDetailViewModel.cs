using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Ehrungsprogramm.Core.Models;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Contracts.Services;
using Ehrungsprogramm.Contracts.ViewModels;

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

        public PersonDetailViewModel()
        {
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
