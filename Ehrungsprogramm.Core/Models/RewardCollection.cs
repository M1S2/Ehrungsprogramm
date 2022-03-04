using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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

        private List<Reward> _rewards = new List<Reward>();
        /// <summary>
        /// List with all rewards
        /// </summary>
        public List<Reward> Rewards
        {
            get => _rewards;
            set => SetProperty(ref _rewards, value);
        }

        /// <summary>
        /// Indexer for this <see cref="RewardCollection"/>
        /// </summary>
        /// <param name="rewardType">Type of the reward to get of set</param>
        /// <returns>Found <see cref="Reward"/> of null</returns>
        public Reward this[RewardTypes rewardType]
        {
            get => Rewards.Where(r => r.Type == rewardType).FirstOrDefault();
            set
            {
                Rewards.RemoveAll(r => r.Type == value.Type);
                Rewards.Add(value);
            }
        }

        /// <summary>
        /// Add all possible rewards to the reward dictionary
        /// </summary>
        public RewardCollection()
        {
            foreach(RewardTypes rewardType in Enum.GetValues(typeof(RewardTypes)).Cast<RewardTypes>())
            {
                Rewards.Add(new Reward() { Type = rewardType });
            }
            Rewards.RemoveAll(r => r.Type == RewardTypes.UNKNOWN);      // Remove the UNKNOWN reward again
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
                this[reward.Type] = reward;

                //Rewards.RemoveAll(r => r.Type == reward.Type);
                //Rewards.Add(reward);
                return true;
            }
            return false;
        }

        public Reward BLSV20 => this[RewardTypes.BLSV20];
        public Reward BLSV25 => this[RewardTypes.BLSV25];
        public Reward BLSV30 => this[RewardTypes.BLSV30];
        public Reward BLSV40 => this[RewardTypes.BLSV40];
        public Reward BLSV45 => this[RewardTypes.BLSV45];
        public Reward BLSV50 => this[RewardTypes.BLSV50];
        public Reward BLSV60 => this[RewardTypes.BLSV60];
        public Reward BLSV70 => this[RewardTypes.BLSV70];
        public Reward BLSV80 => this[RewardTypes.BLSV80];

        public Reward TSVSilver => this[RewardTypes.TSVSILVER];
        public Reward TSVGold => this[RewardTypes.TSVGOLD];
        public Reward TSVHonorary => this[RewardTypes.TSVHONORARY];

        /// <summary>
        /// Contains the highest BLSV reward that is available but not obtained. If not matching reward is found, null is returned.
        /// </summary>
        public Reward HighestAvailableBLSVReward
        {
            get
            {
                Rewards = Rewards.OrderBy(r => r.Type).ToList();
                Reward highestBLSVReward = null;
                foreach(Reward reward in Rewards)
                {
                    if (reward.IsBLSVType && reward.Available && !reward.Obtained)
                    {
                        highestBLSVReward = reward;
                    }
                    else if(reward.IsBLSVType && reward.Available && reward.Obtained)
                    {
                        highestBLSVReward = null;
                    }
                }
                return highestBLSVReward;
            }
        }

        /// <summary>
        /// Contains the highest TSV reward that is available but not obtained. If not matching reward is found, null is returned.
        /// </summary>
        public Reward HighestAvailableTSVReward
        {
            get
            {
                Rewards = Rewards.OrderBy(r => r.Type).ToList();
                Reward highestTSVReward = null;
                foreach (Reward reward in Rewards)
                {
                    if (reward.IsTSVType && reward.Available && !reward.Obtained)
                    {
                        highestTSVReward = reward;
                    }
                    else if (reward.IsTSVType && reward.Available && reward.Obtained)
                    {
                        highestTSVReward = null;
                    }
                }
                return highestTSVReward;
            }
        }
    }
}
