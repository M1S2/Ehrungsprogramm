using System;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public class RewardsBLSVViewModel : ObservableObject, INavigationAware
    {
        #region Visible Items Flags BLSV
        private bool _showBLSV20 = true;
        public bool ShowBLSV20
        {
            get => _showBLSV20;
            set { SetProperty(ref _showBLSV20, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        private bool _showBLSV25 = true;
        public bool ShowBLSV25
        {
            get => _showBLSV25;
            set { SetProperty(ref _showBLSV25, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        private bool _showBLSV30 = true;
        public bool ShowBLSV30
        {
            get => _showBLSV30;
            set { SetProperty(ref _showBLSV30, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        private bool _showBLSV40 = true;
        public bool ShowBLSV40
        {
            get => _showBLSV40;
            set { SetProperty(ref _showBLSV40, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        private bool _showBLSV45 = true;
        public bool ShowBLSV45
        {
            get => _showBLSV45;
            set { SetProperty(ref _showBLSV45, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        private bool _showBLSV50 = true;
        public bool ShowBLSV50
        {
            get => _showBLSV50;
            set { SetProperty(ref _showBLSV50, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        private bool _showBLSV60 = true;
        public bool ShowBLSV60
        {
            get => _showBLSV60;
            set { SetProperty(ref _showBLSV60, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        private bool _showBLSV70 = true;
        public bool ShowBLSV70
        {
            get => _showBLSV70;
            set { SetProperty(ref _showBLSV70, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        private bool _showBLSV80 = true;
        public bool ShowBLSV80
        {
            get => _showBLSV80;
            set { SetProperty(ref _showBLSV80, value); ShowFlagsBLSVSubject.OnNext(true); OnPropertyChanged(nameof(ShowOnlyBLSV25AndBLSV40)); }
        }

        /// <summary>
        /// Property used to indicate to show only the BLSV25 and BLSV40 rewards
        /// </summary>
        public bool ShowOnlyBLSV25AndBLSV40
        {
            get => (!ShowBLSV20 && ShowBLSV25 && !ShowBLSV30 && ShowBLSV40 && !ShowBLSV45 && !ShowBLSV50 && !ShowBLSV60 && !ShowBLSV70 && !ShowBLSV80);
            set
            {
                if (value == true)
                {
                    ShowBLSV20 = false;
                    ShowBLSV25 = true;
                    ShowBLSV30 = false;
                    ShowBLSV40 = true;
                    ShowBLSV45 = false;
                    ShowBLSV50 = false;
                    ShowBLSV60 = false;
                    ShowBLSV70 = false;
                    ShowBLSV80 = false;
                }
                else
                {
                    ShowBLSV20 = true;
                    ShowBLSV25 = true;
                    ShowBLSV30 = true;
                    ShowBLSV40 = true;
                    ShowBLSV45 = true;
                    ShowBLSV50 = true;
                    ShowBLSV60 = true;
                    ShowBLSV70 = true;
                    ShowBLSV80 = true;
                }
            }
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

        public Subject<bool> ShowFlagsBLSVSubject = new Subject<bool>();        // Subject used to update the collection view when the visible items flags change (with a little delay)


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
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog() { FileName = Properties.Resources.DefaultFileNameBlsvRewardOverview, Filter = Properties.Resources.FileFilterPDF };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                { 
                    List<Person> blsvRewardCollection = PeopleItemsBLSVRewardsCollectionView.Cast<Person>().ToList();

                    int numberBlsvRewardsUnfiltered = People.Where(p => p.Rewards.HighestAvailableBLSVReward != null).Count();
                    
                    List<string> filterTextsBlsv = new List<string>();
                    if (!ShowBLSV20) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV20); }
                    if (!ShowBLSV25) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV25); }
                    if (!ShowBLSV30) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV30); }
                    if (!ShowBLSV40) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV40); }
                    if (!ShowBLSV45) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV45); }
                    if (!ShowBLSV50) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV50); }
                    if (!ShowBLSV60) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV60); }
                    if (!ShowBLSV70) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV70); }
                    if (!ShowBLSV80) { filterTextsBlsv.Add(Properties.Enums.RewardTypes_BLSV80); }

                    await _printService?.PrintBlsvRewards(blsvRewardCollection, saveFileDialog.FileName, numberBlsvRewardsUnfiltered, string.Join(", ", filterTextsBlsv));
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

        public RewardsBLSVViewModel(IPersonService personService, IPrintService printService, INavigationService navigationService, IDialogCoordinator dialogCoordinator)
        {
            _personService = personService;
            _printService = printService;
            _navigationService = navigationService;
            _dialogCoordinator = dialogCoordinator;
            People = new List<Person>();

            PeopleItemsBLSVRewardsCollectionView = new CollectionViewSource() { Source = People }.View;
            using (PeopleItemsBLSVRewardsCollectionView.DeferRefresh())
            {
                PeopleItemsBLSVRewardsCollectionView.Filter += (item) =>
                {
                    Reward highestBlsvReward = ((Person)item)?.Rewards?.HighestAvailableBLSVReward;
                    return highestBlsvReward != null && ((highestBlsvReward?.Type == RewardTypes.BLSV20 && ShowBLSV20) ||
                                                        (highestBlsvReward?.Type == RewardTypes.BLSV25 && ShowBLSV25) ||
                                                        (highestBlsvReward?.Type == RewardTypes.BLSV30 && ShowBLSV30) ||
                                                        (highestBlsvReward?.Type == RewardTypes.BLSV40 && ShowBLSV40) ||
                                                        (highestBlsvReward?.Type == RewardTypes.BLSV45 && ShowBLSV45) ||
                                                        (highestBlsvReward?.Type == RewardTypes.BLSV50 && ShowBLSV50) ||
                                                        (highestBlsvReward?.Type == RewardTypes.BLSV60 && ShowBLSV60) ||
                                                        (highestBlsvReward?.Type == RewardTypes.BLSV70 && ShowBLSV70) ||
                                                        (highestBlsvReward?.Type == RewardTypes.BLSV80 && ShowBLSV80));
                };
                PeopleItemsBLSVRewardsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Rewards.HighestAvailableBLSVReward.Type"));
                PeopleItemsBLSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Rewards.HighestAvailableBLSVReward.Type", ListSortDirection.Ascending));
                PeopleItemsBLSVRewardsCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }

            // Use System.Reactive to update the collection views not for each change of the Show... flags but after a defined timespan
            ShowFlagsBLSVSubject.Throttle(TimeSpan.FromMilliseconds(500)).ObserveOn(SynchronizationContext.Current).Subscribe((b) => PeopleItemsBLSVRewardsCollectionView.Refresh());
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
            PeopleItemsBLSVRewardsCollectionView.Refresh();
        }
    }
}
