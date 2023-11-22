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
using System.Reactive.Subjects;
using System.Threading;
using System.Reactive.Linq;
using System.Globalization;

namespace Ehrungsprogramm.ViewModels
{
    public class RewardsTSVViewModel : ObservableObject, INavigationAware
    {
        #region Visible Items Flags TSV
        private bool _showTSVSilver = true;
        public bool ShowTSVSilver
        {
            get => _showTSVSilver;
            set { SetProperty(ref _showTSVSilver, value); ShowFlagsTSVSubject.OnNext(true); }
        }

        private bool _showTSVGold = true;
        public bool ShowTSVGold
        {
            get => _showTSVGold;
            set { SetProperty(ref _showTSVGold, value); ShowFlagsTSVSubject.OnNext(true); }
        }

        private bool _showTSVHonorary = true;
        public bool ShowTSVHonorary
        {
            get => _showTSVHonorary;
            set { SetProperty(ref _showTSVHonorary, value); ShowFlagsTSVSubject.OnNext(true); }
        }
        #endregion

        private List<Person> _people;
        /// <summary>
        /// Collection with all people
        /// </summary>
        public List<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        public Subject<bool> ShowFlagsTSVSubject = new Subject<bool>();         // Subject used to update the collection view when the visible items flags change (with a little delay)


        /// <summary>
        /// View used to display all rewards grouped and filtered based on TSV rewards
        /// </summary>
        public ICollectionView PeopleItemsTSVRewardsCollectionView { get; private set; }


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
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog() { FileName = Properties.Resources.DefaultFileNameTsvRewardOverview, Filter = Properties.Resources.FileFilterPDF };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                { 
                    List<Person> tsvRewardCollection = PeopleItemsTSVRewardsCollectionView.Cast<Person>().ToList();
                   
                    int numberTsvRewardsUnfiltered = People.Where(p => p.Rewards.HighestAvailableTSVReward != null).Count();
                    
                    List<string> filterTextsTsv = new List<string>();
                    if (!ShowTSVSilver) { filterTextsTsv.Add(Properties.Enums.RewardTypes_TSVSILVER); }
                    if (!ShowTSVGold) { filterTextsTsv.Add(Properties.Enums.RewardTypes_TSVGOLD); }
                    if (!ShowTSVHonorary) { filterTextsTsv.Add(Properties.Enums.RewardTypes_TSVHONORARY); }
                   
                    await _printService?.PrintTsvRewards(tsvRewardCollection, saveFileDialog.FileName, numberTsvRewardsUnfiltered, string.Join(", ", filterTextsTsv));
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

        public RewardsTSVViewModel(IPersonService personService, IPrintService printService, INavigationService navigationService, IDialogCoordinator dialogCoordinator)
        {
            _personService = personService;
            _printService = printService;
            _navigationService = navigationService;
            _dialogCoordinator = dialogCoordinator;
            People = new List<Person>();

            PeopleItemsTSVRewardsCollectionView = new CollectionViewSource() { Source = People }.View;
            using (PeopleItemsTSVRewardsCollectionView.DeferRefresh())
            {
                PeopleItemsTSVRewardsCollectionView.Filter += (item) =>
                {
                    Reward highestTsvReward = ((Person)item)?.Rewards?.HighestAvailableTSVReward;
                    return highestTsvReward != null && ((highestTsvReward?.Type == RewardTypes.TSVSILVER && ShowTSVSilver) || 
                                                        (highestTsvReward?.Type == RewardTypes.TSVGOLD && ShowTSVGold) || 
                                                        (highestTsvReward?.Type == RewardTypes.TSVHONORARY && ShowTSVHonorary));
                };
                PeopleItemsTSVRewardsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Rewards.HighestAvailableTSVReward.Type"));
                PeopleItemsTSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Rewards.HighestAvailableTSVReward.Type", ListSortDirection.Ascending));
                PeopleItemsTSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }

            // Use System.Reactive to update the collection views not for each change of the Show... flags but after a defined timespan
            ShowFlagsTSVSubject.Throttle(TimeSpan.FromMilliseconds(250)).ObserveOn(SynchronizationContext.Current).Subscribe((b) => PeopleItemsTSVRewardsCollectionView.Refresh());
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
            PeopleItemsTSVRewardsCollectionView.Refresh();
        }
    }
}
