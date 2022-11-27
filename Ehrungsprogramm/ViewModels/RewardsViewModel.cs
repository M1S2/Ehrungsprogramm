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
using MahApps.Metro.Controls.Dialogs;

namespace Ehrungsprogramm.ViewModels
{
    public class RewardsViewModel : ObservableObject, INavigationAware
    {
        private List<Person> _people;
        /// <summary>
        /// Collection with all people
        /// </summary>
        public List<Person> People
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

        private ICommand _manageDatabaseCommand;
        /// <summary>
        /// Command used to navigate to the ManageDatabasePage
        /// </summary>
        public ICommand ManageDatabaseCommand => _manageDatabaseCommand ?? (_manageDatabaseCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(ManageDatabaseViewModel).FullName)));


        private bool _isPrinting;
        /// <summary>
        /// True, if printing reward overview. False if not currently printing.
        /// </summary>
        public bool IsPrinting
        {
            get => _isPrinting;
            set { SetProperty(ref _isPrinting, value); ((RelayCommand)PrintCommand).NotifyCanExecuteChanged(); }
        }

        private ICommand _printCommand;
        /// <summary>
        /// Command used to print a reward overview.
        /// </summary>
        public ICommand PrintCommand => _printCommand ?? (_printCommand = new RelayCommand(async () =>
        {
            try
            {
                IsPrinting = true;
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog() { FileName = Properties.Resources.DefaultFileNameRewardOverview, Filter = Properties.Resources.FileFilterPDF };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    await _printService?.PrintRewards(People, saveFileDialog.FileName);
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


        private IPersonService _personService;
        private IPrintService _printService;
        private INavigationService _navigationService;
        private IDialogCoordinator _dialogCoordinator;

        public RewardsViewModel(IPersonService personService, IPrintService printService, INavigationService navigationService, IDialogCoordinator dialogCoordinator)
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

            PeopleItemsTSVRewardsCollectionView = new CollectionViewSource() { Source = People }.View;
            using (PeopleItemsTSVRewardsCollectionView.DeferRefresh())
            {
                PeopleItemsTSVRewardsCollectionView.Filter += (item) => ((Person)item).Rewards.HighestAvailableTSVReward != null;
                PeopleItemsTSVRewardsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Rewards.HighestAvailableTSVReward.Type"));
                PeopleItemsTSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Rewards.HighestAvailableTSVReward.Type", ListSortDirection.Ascending));
                PeopleItemsTSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }

            PeopleItemsBLSVRewardsCollectionView = new CollectionViewSource() { Source = People }.View;
            using (PeopleItemsBLSVRewardsCollectionView.DeferRefresh())
            {
                PeopleItemsBLSVRewardsCollectionView.Filter += (item) => ((Person)item).Rewards.HighestAvailableBLSVReward != null;
                PeopleItemsBLSVRewardsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Rewards.HighestAvailableBLSVReward.Type"));
                PeopleItemsBLSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Rewards.HighestAvailableBLSVReward.Type", ListSortDirection.Ascending));
                PeopleItemsBLSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }

            OnPropertyChanged(nameof(People));
        }
    }
}
