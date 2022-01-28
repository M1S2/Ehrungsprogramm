using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Ehrungsprogramm.Core.Models
{
    public enum RewardType
    {
        BLSV40,
        BLSV50,
        BLSV60,
        BLSV70,
        BLSV80,
        TSVSilver,
        TSVGold,
        TSVHonorary
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
            set => SetProperty(ref _type, value);
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
            Rewards.Add(RewardType.BLSV40, new Reward() { Type = RewardType.BLSV40 });
            Rewards.Add(RewardType.BLSV50, new Reward() { Type = RewardType.BLSV50 });
            Rewards.Add(RewardType.BLSV60, new Reward() { Type = RewardType.BLSV60 });
            Rewards.Add(RewardType.BLSV70, new Reward() { Type = RewardType.BLSV70 });
            Rewards.Add(RewardType.BLSV80, new Reward() { Type = RewardType.BLSV80 });
        }

        public Reward BLSV40 => Rewards[RewardType.BLSV40];
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
            Rewards.Add(RewardType.TSVSilver, new Reward() { Type = RewardType.TSVSilver });
            Rewards.Add(RewardType.TSVGold, new Reward() { Type = RewardType.TSVGold });
            Rewards.Add(RewardType.TSVHonorary, new Reward() { Type = RewardType.TSVHonorary });
        }

        public Reward TSVSilver => Rewards[RewardType.TSVSilver];
        public Reward TSVGold => Rewards[RewardType.TSVGold];
        public Reward TSVHonorary => Rewards[RewardType.TSVHonorary];
    }

}
