using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Ehrungsprogramm.Core.Models
{
    public class Function : ObservableObject
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private FunctionType _type;
        public FunctionType Type 
        {
            get => _type; 
            set => SetProperty(ref _type, value);
        }

        private string _description;
        public string Description 
        {
            get => _description; 
            set => SetProperty(ref _description, value);
        }

        private DateTimeRange _timePeriod = new DateTimeRange();
        public DateTimeRange TimePeriod 
        {
            get => _timePeriod;
            set { SetProperty(ref _timePeriod, value); OnPropertyChanged(nameof(FunctionYears)); }
        }

        public int FunctionYears => (int)Math.Ceiling(TimePeriod.Duration.TotalDays / 365);

        private List<DateTimeRange> _effectiveScoreTimePeriods = new List<DateTimeRange>();
        public List<DateTimeRange> EffectiveScoreTimePeriods
        {
            get => _effectiveScoreTimePeriods;
            set { SetProperty(ref _effectiveScoreTimePeriods, value); OnPropertyChanged(nameof(EffectiveScoreYears)); }
        }

        public int EffectiveScoreYears
        {
            get
            {
                double effectiveSum = 0;
                EffectiveScoreTimePeriods.ForEach(p => effectiveSum += p?.Duration.TotalDays ?? 0);
                effectiveSum = Math.Ceiling(effectiveSum / 365);
                return (int)effectiveSum;
            }
        }
        
    }
}
