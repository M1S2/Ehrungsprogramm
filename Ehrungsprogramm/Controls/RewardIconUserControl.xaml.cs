using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ehrungsprogramm.Core.Models;

namespace Ehrungsprogramm.Controls
{
    /// <summary>
    /// Interaktionslogik für RewardIconUserControl.xaml
    /// </summary>
    public partial class RewardIconUserControl : UserControl
    {
        /// <summary>
        /// Reward for which the icon is generated
        /// </summary>
        public Reward Reward
        {
            get { return (Reward)GetValue(RewardProperty); }
            set { SetValue(RewardProperty, value); }
        }
        public static readonly DependencyProperty RewardProperty = DependencyProperty.Register(nameof(Reward), typeof(Reward), typeof(RewardIconUserControl));

        /// <summary>
        /// Text that is displayed in the icon.
        /// For BLSV Rewards this is the number of years of the Reward.
        /// For TSV Rewards this is "S" for Silver, "G" for Gold or "H" for Honorary.
        /// </summary>
        public string IconNumberText
        {
            get
            {
                if (Reward.IsBLSVType)
                {
                    return ((int)(Reward.Type)).ToString();
                }
                else if(Reward.IsTSVType)
                {
                    switch(Reward.Type)
                    {
                        case RewardType.TSVSILVER: return "S";
                        case RewardType.TSVGOLD: return "G";
                        case RewardType.TSVHONORARY: return "H";
                        default: return "?";
                    }
                }
                return "?";
            }
        }

        /// <summary>
        /// Constructor of the RewardIconUserControl
        /// </summary>
        public RewardIconUserControl()
        {
            InitializeComponent();
        }
    }
}
