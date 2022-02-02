using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Ehrungsprogramm.Core.Models
{
    /// <summary>
    /// Class holding a collection of all available rewards
    /// </summary>
    public class RewardCollection : ObservableObject
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
        /// <summary>
        /// Dictionary with all rewards
        /// </summary>
        public Dictionary<RewardType, Reward> Rewards
        {
            get => _rewards;
            set => SetProperty(ref _rewards, value);
        }

        /// <summary>
        /// Add all possible rewards to the reward dictionary
        /// </summary>
        public RewardCollection()
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

            Rewards.Add(RewardType.TSVSILVER, new Reward() { Type = RewardType.TSVSILVER });
            Rewards.Add(RewardType.TSVGOLD, new Reward() { Type = RewardType.TSVGOLD });
            Rewards.Add(RewardType.TSVHONORARY, new Reward() { Type = RewardType.TSVHONORARY });
        }

        /// <summary>
        /// Updates a reward in the dictionary of rewards
        /// </summary>
        /// <param name="reward">Reward to update</param>
        /// <returns>false for UNKNOWN reward type; otherwise true</returns>
        public bool AddReward(Reward reward)
        {
            if (reward.IsBLSVType || reward.IsTSVType)
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

        public Reward TSVSilver => Rewards[RewardType.TSVSILVER];
        public Reward TSVGold => Rewards[RewardType.TSVGOLD];
        public Reward TSVHonorary => Rewards[RewardType.TSVHONORARY];
    }
}
