using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Ehrungsprogramm.Core.Models
{
    /// <summary>
    /// Class describing a person.
    /// </summary>
    public class Person : ObservableObject, IEquatable<Person>
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

        private int _membershipYears;
        /// <summary>
        /// Number of years, the person was in the sports club. This is calculated from the <see cref="EntryDate"/> property and is rounded up to the next full number of years.
        /// </summary>
        public int MembershipYears
        {
            get => _membershipYears;
            set => SetProperty(ref _membershipYears, value);
        }

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

        private int _scoreTSVFunctions;
        /// <summary>
        /// Number of points for the TSV functions.
        /// </summary>
        public int ScoreTSVFunctions
        {
            get => _scoreTSVFunctions;
            set => SetProperty(ref _scoreTSVFunctions, value);
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

        private RewardCollection _rewards = new RewardCollection();
        /// <summary>
        /// Collection with all rewards of the person (available and already obtained)
        /// </summary>
        public RewardCollection Rewards
        {
            get => _rewards;
            set => SetProperty(ref _rewards, value);
        }

        private string _parsingFailureMessage;
        /// <summary>
        /// If an error occured while parsing this person, this string holds a detailed message of the error.
        /// If no error occured, this string is empty.
        /// </summary>
        public string ParsingFailureMessage
        {
            get => _parsingFailureMessage;
            set => SetProperty(ref _parsingFailureMessage, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two Persons are equal
        /// </summary>
        /// <param name="obj">Other Person to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="Person"/></returns>
        public override bool Equals(object obj)
        {
            Person other = obj as Person;
            if (other == null) return false;

            return Name.Equals(other.Name) &&
                FirstName.Equals(other.FirstName) &&
                BirthDate.Equals(other.BirthDate) &&
                EntryDate.Equals(other.EntryDate);
        }

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(Person other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Id;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return FirstName + ", " + Name + " (Birth: " + BirthDate.ToShortDateString() + ")";
        }
    }
}
