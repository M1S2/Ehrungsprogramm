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
        #region Visible Items Flags TSV
        private bool _showTSVSilver = true;
        public bool ShowTSVSilver
        {
            get => _showTSVSilver;
            set { SetProperty(ref _showTSVSilver, value); PeopleItemsTSVRewardsCollectionView.Refresh(); }
        }

        private bool _showTSVGold = true;
        public bool ShowTSVGold
        {
            get => _showTSVGold;
            set { SetProperty(ref _showTSVGold, value); PeopleItemsTSVRewardsCollectionView.Refresh(); }
        }

        private bool _showTSVHonorary = true;
        public bool ShowTSVHonorary
        {
            get => _showTSVHonorary;
            set { SetProperty(ref _showTSVHonorary, value); PeopleItemsTSVRewardsCollectionView.Refresh(); }
        }
        #endregion

        #region Visible Items Flags BLSV
        private bool _showBLSV20 = true;
        public bool ShowBLSV20
        {
            get => _showBLSV20;
            set { SetProperty(ref _showBLSV20, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
        }

        private bool _showBLSV25 = true;
        public bool ShowBLSV25
        {
            get => _showBLSV25;
            set { SetProperty(ref _showBLSV25, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
        }

        private bool _showBLSV30 = true;
        public bool ShowBLSV30
        {
            get => _showBLSV30;
            set { SetProperty(ref _showBLSV30, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
        }

        private bool _showBLSV40 = true;
        public bool ShowBLSV40
        {
            get => _showBLSV40;
            set { SetProperty(ref _showBLSV40, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
        }

        private bool _showBLSV45 = true;
        public bool ShowBLSV45
        {
            get => _showBLSV45;
            set { SetProperty(ref _showBLSV45, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
        }

        private bool _showBLSV50 = true;
        public bool ShowBLSV50
        {
            get => _showBLSV50;
            set { SetProperty(ref _showBLSV50, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
        }

        private bool _showBLSV60 = true;
        public bool ShowBLSV60
        {
            get => _showBLSV60;
            set { SetProperty(ref _showBLSV60, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
        }

        private bool _showBLSV70 = true;
        public bool ShowBLSV70
        {
            get => _showBLSV70;
            set { SetProperty(ref _showBLSV70, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
        }

        private bool _showBLSV80 = true;
        public bool ShowBLSV80
        {
            get => _showBLSV80;
            set { SetProperty(ref _showBLSV80, value); PeopleItemsBLSVRewardsCollectionView.Refresh(); }
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
                    List<Person> tsvRewardCollection = PeopleItemsTSVRewardsCollectionView.Cast<Person>().ToList();
                    List<Person> blsvRewardCollection = PeopleItemsBLSVRewardsCollectionView.Cast<Person>().ToList();
                    await _printService?.PrintRewards(tsvRewardCollection, blsvRewardCollection, saveFileDialog.FileName);
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
            PeopleItemsBLSVRewardsCollectionView.Refresh();
        }
    }
}
