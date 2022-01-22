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

        private DateTimeRange _timePeriod;
        public DateTimeRange TimePeriod 
        {
            get => _timePeriod;
            set { SetProperty(ref _timePeriod, value); OnPropertyChanged(nameof(FunctionYears)); }
        }

        public int FunctionYears => (int)Math.Ceiling(TimePeriod.Duration.TotalDays / 365);

        private DateTimeRange _effectiveScoreTimePeriod1;
        public DateTimeRange EffectiveScoreTimePeriod1
        {
            get => _effectiveScoreTimePeriod1;
            set { SetProperty(ref _effectiveScoreTimePeriod1, value); OnPropertyChanged(nameof(EffectiveScoreYears)); }
        }

        private DateTimeRange _effectiveScoreTimePeriod2;
        public DateTimeRange EffectiveScoreTimePeriod2
        {
            get => _effectiveScoreTimePeriod2;
            set { SetProperty(ref _effectiveScoreTimePeriod2, value); OnPropertyChanged(nameof(EffectiveScoreYears)); }
        }

        public int EffectiveScoreYears
        {
            get
            {
                double effectiveSum = EffectiveScoreTimePeriod1?.Duration.TotalDays ?? 0;
                effectiveSum += EffectiveScoreTimePeriod2?.Duration.TotalDays ?? 0;
                effectiveSum = Math.Ceiling(effectiveSum / 365);
                return (int)effectiveSum;
            }
        }
        

        public Function()
        {
            TimePeriod = new DateTimeRange();
        }
    }
}
