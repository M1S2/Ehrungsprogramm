using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Itenso.TimePeriod;

namespace Ehrungsprogramm.Core.Models
{
    /// <summary>
    /// Class that is describing a function that a person has.
    /// This can be a job as board member, head of an departement
    /// or any other function like trainer or press officer.
    /// </summary>
    public class Function : ObservableObject
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

        /// <summary>
        /// Number of years, the function was done. This is calculated from the <see cref="TimePeriod"/> property and is rounded up to the next full number of years.
        /// </summary>
        public int FunctionYears => (int)Math.Ceiling(TimePeriod.Duration.TotalDays / 365);        
    }
}
