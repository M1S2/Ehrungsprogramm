using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Itenso.TimePeriod;

namespace Ehrungsprogramm.Core.Models
{
    /// <summary>
    /// Available function types.
    /// </summary>
    public enum FunctionType
    {
        /// <summary>
        /// Job as board member
        /// </summary>
        BOARD_MEMBER,

        /// <summary>
        /// Role as head of departement
        /// </summary>
        HEAD_OF_DEPARTEMENT,

        /// <summary>
        /// Any other function like trainer or press officer.
        /// </summary>
        OTHER_FUNCTION,

        /// <summary>
        /// Unknown function. This should only be used as placeholder.
        /// </summary>
        UNKNOWN
    }


    /// <summary>
    /// Class that is describing a function that a person has.
    /// This can be a job as board member, head of an departement
    /// or any other function like trainer or press officer.
    /// </summary>
    public class Function : ObservableObject, IEquatable<Function>
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

        private FunctionType _type;
        /// <summary>
        /// Type of the function.
        /// </summary>
        public FunctionType Type 
        {
            get => _type; 
            set => SetProperty(ref _type, value);
        }

        private string _description;
        /// <summary>
        /// String describing the function. This can also contain a more precise description of the function type than the <see cref="Type"/> property.
        /// </summary>
        public string Description 
        {
            get => _description; 
            set => SetProperty(ref _description, value);
        }

        private TimeRange _timePeriod = new TimeRange();
        /// <summary>
        /// Time range in which the function was done.
        /// </summary>
        public TimeRange TimePeriod 
        {
            get => _timePeriod;
            set { SetProperty(ref _timePeriod, value); OnPropertyChanged(nameof(FunctionYears)); }
        }

        private bool _isFunctionOngoing;
        /// <summary>
        /// True, if the function isn't ended yet.
        /// </summary>
        public bool IsFunctionOngoing
        {
            get => _isFunctionOngoing;
            set { SetProperty(ref _isFunctionOngoing, value); }
        }

        /// <summary>
        /// Number of years, the function was done. This is calculated from the <see cref="TimePeriod"/> property and is rounded up to the next full number of years.
        /// </summary>
        public int FunctionYears => (int)Math.Ceiling(TimePeriod.Duration.TotalDays / 365);

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two Functions are equal
        /// </summary>
        /// <param name="obj">Other Function to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="Function"/></returns>
        public override bool Equals(object obj)
        {
            Function other = obj as Function;
            if (other == null) return false;

            return Type.Equals(other.Type) &&
                Description.Equals(other.Description) &&
                TimePeriod.Equals(other.TimePeriod);
        }

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(Function other)
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
            return Type.ToString() + ": " + Description + " (" + TimePeriod.ToString() + ")";
        }
    }
}
