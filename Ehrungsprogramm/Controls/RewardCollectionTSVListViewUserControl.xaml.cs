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
    /// Interaktionslogik für RewardCollectionTSVListViewUserControl.xaml
    /// </summary>
    public partial class RewardCollectionTSVListViewUserControl : UserControl
    {
        public RewardTSVCollection Rewards
        {
            get { return (RewardTSVCollection)GetValue(RewardsProperty); }
            set { SetValue(RewardsProperty, value); }
        }
        public static readonly DependencyProperty RewardsProperty = DependencyProperty.Register(nameof(Rewards), typeof(RewardTSVCollection), typeof(RewardCollectionTSVListViewUserControl));

        public RewardCollectionTSVListViewUserControl()
        {
            InitializeComponent();
        }
    }
}
