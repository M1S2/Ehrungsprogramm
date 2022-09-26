using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class RewardIconUserControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Reward for which the icon is generated
        /// </summary>
        public Reward Reward
        {
            get { return (Reward)GetValue(RewardProperty); }
            set { SetValue(RewardProperty, value); }
        }
        public static readonly DependencyProperty RewardProperty = DependencyProperty.Register(nameof(Reward), typeof(Reward), typeof(RewardIconUserControl), new PropertyMetadata(RewardChanged));

        private static void RewardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RewardIconUserControl).RaisePropertyChanged(nameof(IconNumberText));
        }

        /// <summary>
        /// Text that is displayed in the icon.
        /// For BLSV Rewards this is the number of years of the Reward.
        /// For TSV Rewards this is Resources.RewardIconSilverString for Silver (e.g. "S"), Resources.RewardIconGoldString for Gold (e.g. "G") or Resources.RewardIconHonoraryString for Honorary (e.g. "H").
        /// </summary>
        public string IconNumberText
        {
            get
            {
                if (Reward != null && Reward.IsBLSVType)
                {
                    return ((int)(Reward.Type)).ToString();
                }
                else if(Reward != null && Reward.IsTSVType)
                {
                    switch(Reward.Type)
                    {
                        case RewardTypes.TSVSILVER: return Properties.Resources.RewardIconSilverString;
                        case RewardTypes.TSVGOLD: return Properties.Resources.RewardIconGoldString;
                        case RewardTypes.TSVHONORARY: return Properties.Resources.RewardIconHonoraryString;
                        default: return "?";
                    }
                }
                return "+";
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
