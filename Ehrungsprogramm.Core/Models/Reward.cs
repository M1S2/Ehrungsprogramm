using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Ehrungsprogramm.Core.Models
{
    /// <summary>
    /// Available reward types
    /// </summary>
    public enum RewardTypes
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


    /// <summary>
    /// Class describing a reward
    /// </summary>
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

        private RewardTypes _type;
        /// <summary>
        /// Type of the reward
        /// </summary>
        public RewardTypes Type 
        { 
            get => _type;
            set { SetProperty(ref _type, value); OnPropertyChanged(nameof(IsBLSVType)); OnPropertyChanged(nameof(IsTSVType)); }
        }

        /// <summary>
        /// Is the reward a BLSV reward
        /// </summary>
        public bool IsBLSVType
        {
            get
            {
                switch (Type)
                {
                    case RewardTypes.BLSV20:
                    case RewardTypes.BLSV25:
                    case RewardTypes.BLSV30:
                    case RewardTypes.BLSV40:
                    case RewardTypes.BLSV45:
                    case RewardTypes.BLSV50:
                    case RewardTypes.BLSV60:
                    case RewardTypes.BLSV70:
                    case RewardTypes.BLSV80:
                        return true;
                    default: return false;
                }
            }
        }

        /// <summary>
        /// Is the reward a TSV reward
        /// </summary>
        public bool IsTSVType
        {
            get
            {
                switch (Type)
                {
                    case RewardTypes.TSVSILVER:
                    case RewardTypes.TSVGOLD:
                    case RewardTypes.TSVHONORARY:
                        return true;
                    default: return false;
                }
            }
        }

        private string _description;
        /// <summary>
        /// Description string for the reward which contains details informations
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private bool _available;
        /// <summary>
        /// Indicating if the reward is available (if the corresponding score is high enough). The reward hasn't to be obtained yet.
        /// </summary>
        public bool Available
        {
            get => _available;
            set => SetProperty(ref _available, value);
        }

        private bool _obtained;
        /// <summary>
        /// Indicating if the reward was obtained already
        /// </summary>
        public bool Obtained
        {
            get => _obtained;
            set => SetProperty(ref _obtained, value);
        }

        private DateTime _obtainedDate;
        /// <summary>
        /// Date when the reward was obtained (only valid if <see cref="Obtained"/> is true)
        /// </summary>
        public DateTime ObtainedDate
        {
            get => _obtainedDate;
            set => SetProperty(ref _obtainedDate, value);
        }

        /// <summary>
        /// Constructor for the Reward
        /// </summary>
        public Reward()
        {
            Description = "";
            Available = false;
            Obtained = false;
            ObtainedDate = DateTime.MinValue;
        }

        /// <summary>
        /// Override the ToString method.
        /// </summary>
        /// <returns>Return an more readable string for the reward object</returns>
        public override string ToString()
        {
            return Description + " (Type: " + Type.ToString() + ")";
        }
    }

}
