using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Ehrungsprogramm.Core.Models
{
    /// <summary>
    /// Class describing a person.
    /// </summary>
    public class Person : ObservableObject
    {
        private int _id;
        /// <summary>
        /// Id used to identify this object in a database.
        /// </summary>
        public int Id 
        {
            get => _id; 
            set => SetProperty(ref _id, value);
        }

        private string _name;
        /// <summary>
        /// Last name of the person.
        /// </summary>
        public string Name 
        {
            get => _name; 
            set => SetProperty(ref _name, value);
        }

        private string _firstName;
        /// <summary>
        /// First name of the person.
        /// </summary>
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private DateTime _birthDate;
        /// <summary>
        /// Birth date of the person.
        /// </summary>
        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        private DateTime _entryDate;
        /// <summary>
        /// Entry date of the person into the sports club.
        /// </summary>
        public DateTime EntryDate
        {
            get => _entryDate;
            set { SetProperty(ref _entryDate, value); OnPropertyChanged(nameof(MembershipYears)); OnPropertyChanged(nameof(ScoreBLSV)); }
        }

        /// <summary>
        /// Number of years, the person was in the sports club. This is calculated from the <see cref="EntryDate"/> property until now and is rounded up to the next full number of years.
        /// </summary>
        public int MembershipYears => (int)Math.Ceiling((DateTime.Now - EntryDate).TotalDays / 365);

        private int _scoreBLSV;
        /// <summary>
        /// Number of points for the BLSV rewards.
        /// </summary>
        public int ScoreBLSV
        {
            get => _scoreBLSV;
            set => SetProperty(ref _scoreBLSV, value);
        }

        private int _scoreTSV;
        /// <summary>
        /// Number of points for the TSV rewards.
        /// </summary>
        public int ScoreTSV
        {
            get => _scoreTSV;
            set => SetProperty(ref _scoreTSV, value);
        }

        private List<Function> _functions = new List<Function>();
        /// <summary>
        /// List containing all functions that a person has / had.
        /// </summary>
        public List<Function> Functions 
        {
            get => _functions; 
            set { SetProperty(ref _functions, value); }
        }

        private int _effectiveBoardMemberYears;
        /// <summary>
        /// Number of effective years (years that can be rewarded) as board member.
        /// </summary>
        public int EffectiveBoardMemberYears
        {
            get => _effectiveBoardMemberYears;
            set => SetProperty(ref _effectiveBoardMemberYears, value);
        }

        private int _effectiveHeadOfDepartementYears;
        /// <summary>
        /// Number of effective years (years that can be rewarded) as head of departement.
        /// </summary>
        public int EffectiveHeadOfDepartementYears
        {
            get => _effectiveHeadOfDepartementYears;
            set => SetProperty(ref _effectiveHeadOfDepartementYears, value);
        }

        private int _effectiveOtherFunctionsYears;
        /// <summary>
        /// Number of effective years (years that can be rewarded) for any other function.
        /// </summary>
        public int EffectiveOtherFunctionsYears
        {
            get => _effectiveOtherFunctionsYears;
            set => SetProperty(ref _effectiveOtherFunctionsYears, value);
        }
    }
}
