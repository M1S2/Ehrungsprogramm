using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Ehrungsprogramm.Core.Models
{
    public enum RewardType
    {
        BLSV20 = 20,
        BLSV25 = 25,
        BLSV30 = 30,
        BLSV40 = 40,
        BLSV45 = 45,
        BLSV50 = 50,
        BLSV60 = 60,
        BLSV70 = 70,
        BLSV80 = 80,
        TSVSILVER = 101,
        TSVGOLD = 102,
        TSVHONORARY = 103,
        UNKNOWN = 0
    }

    public class Reward : ObservableObject
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

        private RewardType _type;
        public RewardType Type 
        { 
            get => _type;
            set { SetProperty(ref _type, value); OnPropertyChanged(nameof(IsBLSVType)); OnPropertyChanged(nameof(IsTSVType)); }
        }

        public bool IsBLSVType
        {
            get
            {
                switch (Type)
                {
                    case RewardType.BLSV20:
                    case RewardType.BLSV25:
                    case RewardType.BLSV30:
                    case RewardType.BLSV40:
                    case RewardType.BLSV45:
                    case RewardType.BLSV50:
                    case RewardType.BLSV60:
                    case RewardType.BLSV70:
                    case RewardType.BLSV80:
                        return true;
                    default: return false;
                }
            }
        }

        public bool IsTSVType
        {
            get
            {
                switch (Type)
                {
                    case RewardType.TSVSILVER:
                    case RewardType.TSVGOLD:
                    case RewardType.TSVHONORARY:
                        return true;
                    default: return false;
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private bool _available;
        public bool Available
        {
            get => _available;
            set => SetProperty(ref _available, value);
        }

        private bool _obtained;
        public bool Obtained
        {
            get => _obtained;
            set => SetProperty(ref _obtained, value);
        }

        private DateTime _obtainedDate;
        public DateTime ObtainedDate
        {
            get => _obtainedDate;
            set => SetProperty(ref _obtainedDate, value);
        }

        public Reward()
        {
            Available = false;
            Obtained = false;
            ObtainedDate = DateTime.MinValue;
        }
    }

    public class RewardBLSVCollection : ObservableObject
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

        private Dictionary<RewardType, Reward> _rewards = new Dictionary<RewardType, Reward>();
        public Dictionary<RewardType, Reward> Rewards
        {
            get => _rewards;
            set => SetProperty(ref _rewards, value);
        }

        public RewardBLSVCollection()
        {
            Rewards.Add(RewardType.BLSV20, new Reward() { Type = RewardType.BLSV20 });
            Rewards.Add(RewardType.BLSV25, new Reward() { Type = RewardType.BLSV25 });
            Rewards.Add(RewardType.BLSV30, new Reward() { Type = RewardType.BLSV30 });
            Rewards.Add(RewardType.BLSV40, new Reward() { Type = RewardType.BLSV40 });
            Rewards.Add(RewardType.BLSV45, new Reward() { Type = RewardType.BLSV45 });
            Rewards.Add(RewardType.BLSV50, new Reward() { Type = RewardType.BLSV50 });
            Rewards.Add(RewardType.BLSV60, new Reward() { Type = RewardType.BLSV60 });
            Rewards.Add(RewardType.BLSV70, new Reward() { Type = RewardType.BLSV70 });
            Rewards.Add(RewardType.BLSV80, new Reward() { Type = RewardType.BLSV80 });
        }

        public bool AddReward(Reward reward)
        {
            if (reward.IsBLSVType) 
            { 
                Rewards[reward.Type] = reward;
                return true;
            }
            return false;
        }

        public Reward BLSV20 => Rewards[RewardType.BLSV20];
        public Reward BLSV25 => Rewards[RewardType.BLSV25];
        public Reward BLSV30 => Rewards[RewardType.BLSV30];
        public Reward BLSV40 => Rewards[RewardType.BLSV40]; 
        public Reward BLSV45 => Rewards[RewardType.BLSV45];
        public Reward BLSV50 => Rewards[RewardType.BLSV50];
        public Reward BLSV60 => Rewards[RewardType.BLSV60];
        public Reward BLSV70 => Rewards[RewardType.BLSV70];
        public Reward BLSV80 => Rewards[RewardType.BLSV80];
    }

    public class RewardTSVCollection : ObservableObject
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

        private Dictionary<RewardType, Reward> _rewards = new Dictionary<RewardType, Reward>();
        public Dictionary<RewardType, Reward> Rewards
        {
            get => _rewards;
            set => SetProperty(ref _rewards, value);
        }

        public RewardTSVCollection()
        {
            Rewards.Add(RewardType.TSVSILVER, new Reward() { Type = RewardType.TSVSILVER });
            Rewards.Add(RewardType.TSVGOLD, new Reward() { Type = RewardType.TSVGOLD });
            Rewards.Add(RewardType.TSVHONORARY, new Reward() { Type = RewardType.TSVHONORARY });
        }

        public bool AddReward(Reward reward)
        {
            if (reward.IsTSVType)
            {
                Rewards[reward.Type] = reward;
                return true;
            }
            return false;
        }

        public Reward TSVSilver => Rewards[RewardType.TSVSILVER];
        public Reward TSVGold => Rewards[RewardType.TSVGOLD];
        public Reward TSVHonorary => Rewards[RewardType.TSVHONORARY];
    }

}
