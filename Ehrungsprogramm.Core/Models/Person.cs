using System;
using System.Collections.Generic;
using System.Text;
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

        private int _scoreBLSV;
        public int ScoreBLSV
        {
            get => _scoreBLSV;
            set => SetProperty(ref _scoreBLSV, value);
        }

        private int _scoreTSV;
        public int ScoreTSV
        {
            get => _scoreTSV;
            set => SetProperty(ref _scoreTSV, value);
        }

        private List<Function> _functions = new List<Function>();
        public List<Function> Functions 
        {
            get => _functions; 
            set { SetProperty(ref _functions, value); }
        }
    }
}
