using System;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        /// <summary>
        /// Collection with all people
        /// </summary>
        public ObservableCollection<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        /// <summary>
        /// View used to display all rewards grouped and filtered based on TSV rewards
        /// </summary>
        public ICollectionView PeopleItemsTSVRewardsCollectionView { get; private set; }

        /// <summary>
        /// View used to display all rewards grouped and filtered based on BLSV rewards
        /// </summary>
        public ICollectionView PeopleItemsBLSVRewardsCollectionView { get; private set; }


        private ICommand _personDetailCommand;
        /// <summary>
        /// Command used to show details for a specific command
        /// </summary>
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

            PeopleItemsTSVRewardsCollectionView = new CollectionViewSource() { Source = People }.View;
            PeopleItemsTSVRewardsCollectionView.Filter += (item) => ((Person)item).Rewards.HighestAvailableTSVReward != null;
            PeopleItemsTSVRewardsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Rewards.HighestAvailableTSVReward.Type"));
            PeopleItemsTSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Rewards.HighestAvailableTSVReward.Type", ListSortDirection.Ascending));
            PeopleItemsTSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            PeopleItemsBLSVRewardsCollectionView = new CollectionViewSource() { Source = People }.View;
            PeopleItemsBLSVRewardsCollectionView.Filter += (item) => ((Person)item).Rewards.HighestAvailableBLSVReward != null;
            PeopleItemsBLSVRewardsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Rewards.HighestAvailableBLSVReward.Type")); 
            PeopleItemsBLSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Rewards.HighestAvailableBLSVReward.Type", ListSortDirection.Ascending));
            PeopleItemsBLSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }
    }
}
