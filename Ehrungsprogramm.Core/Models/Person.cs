using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Ehrungsprogramm.Core.Models
{
    public class Person : ObservableObject
    {
        private int _id;
        public int Id 
        {
            get => _id; 
            set => SetProperty(ref _id, value);
        }

        private string _name;
        public string Name 
        {
            get => _name; 
            set => SetProperty(ref _name, value);
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        private DateTime _entryDate;
        public DateTime EntryDate
        {
            get => _entryDate;
            set { SetProperty(ref _entryDate, value); OnPropertyChanged(nameof(MembershipYears)); OnPropertyChanged(nameof(ScoreBLSV)); }
        }

        public int MembershipYears => (int)Math.Ceiling((DateTime.Now - EntryDate).TotalDays / 365);

        public int ScoreBLSV => CalculateScoreBLSV();

        private int _scoreTSV;
        public int ScoreTSV
        {
            get => _scoreTSV;
            set => SetProperty(ref _scoreTSV, value);
        }

        private List<Function> _functions;
        public List<Function> Functions 
        {
            get => _functions; 
            set { SetProperty(ref _functions, value); CalculateScoreTSV(); }
        }



        private int CalculateScoreBLSV()
        {
            return MembershipYears * 1;
        }

        private void CalculateScoreTSV()
        {
            ScoreTSV = 5;

            List<Function> functionsBoardMember = Functions.Where(f => f.Type == FunctionType.BOARD_MEMBER).ToList();
            List<Function> functionsHeadOfDepartement = Functions.Where(f => f.Type == FunctionType.HEAD_OF_DEPARTEMENT).ToList();
            List<Function> functionsOther = Functions.Where(f => f.Type == FunctionType.OTHER_FUNCTION).ToList();

            DateTimeRange additionalRange;

            // Loop over all board member function entries and compare them against the other functions
            foreach (Function funcBM in functionsBoardMember)
            {
                funcBM.EffectiveScoreTimePeriod1 = funcBM.TimePeriod;

                foreach(Function funcHead in functionsHeadOfDepartement)
                {
                    funcHead.EffectiveScoreTimePeriod1 = funcHead.TimePeriod.Subtract(funcBM.TimePeriod, out additionalRange);
                    funcHead.EffectiveScoreTimePeriod2 = additionalRange;
                }
                foreach (Function funcOther in functionsOther)
                {
                    funcOther.EffectiveScoreTimePeriod1 = funcOther.TimePeriod.Subtract(funcBM.TimePeriod, out additionalRange);
                    funcOther.EffectiveScoreTimePeriod2 = additionalRange;
                }
            }

            // Loop over all head of departement function entries and compare them against the other functions
            foreach (Function funcHead in functionsHeadOfDepartement)
            {
                foreach (Function funcOther in functionsOther)
                {
                    funcOther.EffectiveScoreTimePeriod1 = funcOther.EffectiveScoreTimePeriod1.Subtract(funcHead.EffectiveScoreTimePeriod1, out additionalRange);
                    if (additionalRange != null) { funcOther.EffectiveScoreTimePeriod2 = additionalRange; }
                }
            }

            OnPropertyChanged(nameof(ScoreTSV));
        }
    }
}
