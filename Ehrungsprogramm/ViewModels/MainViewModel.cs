using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Ehrungsprogramm.Contracts.Services;

namespace Ehrungsprogramm.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ICommand PersonsCommand => _personsCommand ?? (_personsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PersonsViewModel).FullName)));
        public ICommand RewardsTSVCommand => _rewardsTsvCommand ?? (_rewardsTsvCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(RewardsTSVViewModel).FullName)));
        public ICommand RewardsBLSVCommand => _rewardsBlsvCommand ?? (_rewardsBlsvCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(RewardsBLSVViewModel).FullName)));
        public ICommand ManageDatabaseCommand => _manageDatabaseCommand ?? (_manageDatabaseCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(ManageDatabaseViewModel).FullName)));
        public ICommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(SettingsViewModel).FullName)));

        private readonly INavigationService _navigationService;
        private ICommand _personsCommand;
        private ICommand _rewardsTsvCommand;
        private ICommand _rewardsBlsvCommand;
        private ICommand _manageDatabaseCommand;
        private ICommand _settingsCommand;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
