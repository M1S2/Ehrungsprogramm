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
using System.Text.RegularExpressions;
using System.Linq;

namespace Ehrungsprogramm.ViewModels
{
    public class PersonsViewModel : ObservableObject, INavigationAware
    {
        public const string FILTER_FLAG_WARN = "&warn";         // The warn flag is used to only show people with parsing failures

        private List<Person> _people;
        /// <summary>
        /// List of people shown on the person overview page
        /// </summary>
        public List<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        private string _filterText = "";
        /// <summary>
        /// Text used to filter the People list
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set => SetProperty(ref _filterText, value);
        }

        /// <summary>
        /// Function used when filtering the person list
        /// </summary>
        public Func<object, string, bool> PersonFilter
        {
            get
            {
                return (item, text) =>
                {
                    Person person = item as Person;

                    // The filter text can contain multiple additional flags that are marked by & infront of them (e.g. "Name &warn").
                    Regex regexFilterTextFlags = new Regex(@"\&([^&]*)");                       // matching each flag (&...)
                    MatchCollection matches = regexFilterTextFlags.Matches(text);
                    foreach (Match match in matches) { text = text.Replace(match.Value, ""); }  // Remove all flags from the filter text
                    text = text.Trim();

                    bool filterResult = person.Name.ToLower().Contains(text?.ToLower()) 
                        || person.FirstName.ToLower().Contains(text?.ToLower())
                        || (person.FirstName.ToLower() + " " + person.Name.ToLower()).Contains(text?.ToLower())
                        || (person.Name.ToLower() + " " + person.FirstName.ToLower()).Contains(text?.ToLower())
                        || (person.FirstName.ToLower() + ", " + person.Name.ToLower()).Contains(text?.ToLower())
                        || (person.Name.ToLower() + ", " + person.FirstName.ToLower()).Contains(text?.ToLower())
                        || person.ScoreBLSV.ToString().Contains(text ?? "")
                        || person.ScoreTSV.ToString().Contains(text ?? "")
                        || person.EntryDate.ToString().Contains(text ?? "");

                    // Process each flag
                    foreach (Match match in matches)
                    {
                        switch (match.Value.ToLower())
                        {
                            // The warn flag is used to only show people with parsing failures
                            case FILTER_FLAG_WARN: filterResult &= !string.IsNullOrEmpty(person.ParsingFailureMessage); break;
                            default: break;
                        }
                    }

                    return filterResult;
                };
            }
        }

        private ICommand _personDetailCommand;
        /// <summary>
        /// Command used to navigate to the details page of a specific person
        /// </summary>
        public ICommand PersonDetailCommand => _personDetailCommand ?? (_personDetailCommand = new RelayCommand<Person>((person) => _navigationService.NavigateTo(typeof(PersonDetailViewModel).FullName, person)));

        private ICommand _manageDatabaseCommand;
        /// <summary>
        /// Command used to navigate to the ManageDatabasePage
        /// </summary>
        public ICommand ManageDatabaseCommand => _manageDatabaseCommand ?? (_manageDatabaseCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(ManageDatabaseViewModel).FullName)));


        private bool _isPrinting;
        /// <summary>
        /// True, if printing person overview. False if not currently printing.
        /// </summary>
        public bool IsPrinting 
        {
            get => _isPrinting;
            set { SetProperty(ref _isPrinting, value); ((RelayCommand)PrintCommand).NotifyCanExecuteChanged(); }
        }

        private ICommand _printCommand;
        /// <summary>
        /// Command used to print a person overview.
        /// </summary>
        public ICommand PrintCommand => _printCommand ?? (_printCommand = new RelayCommand(async () =>
        {
            try
            {
                IsPrinting = true;
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog() { FileName = "PersonOverview.pdf", Filter = "PDF File|*.pdf" };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    await _printService?.PrintPersonList(People, saveFileDialog.FileName);
                    await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.PrintString, Properties.Resources.PrintString + " " + Properties.Resources.SuccessfulString.ToLower());
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.PrintString, Properties.Resources.ErrorString + ": " + ex.Message);
                }
                catch(Exception)
                {
                    /* Error couldn't be displayed. No action needed here. */
                }
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

        /// <summary>
        /// Method called when navigated to this page
        /// </summary>
        /// <param name="parameter">The parameter is used to set the FilterText when navigated to this page. Use null to leave the FilterText as is.</param>
        public void OnNavigatedTo(object parameter)
        {
            string filterText = parameter as string;
            if (filterText != null) { FilterText = filterText; }

            People.Clear();
            List<Person> servicePeople = _personService?.GetPersons();
            servicePeople?.ForEach(p => People.Add(p));
            OnPropertyChanged(nameof(People));
        }
    }
}
