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
    /// Interaktionslogik für RewardCollectionBLSVListViewUserControl.xaml
    /// </summary>
    public partial class RewardCollectionBLSVListViewUserControl : UserControl
    {
        public RewardBLSVCollection Rewards
        {
            get { return (RewardBLSVCollection)GetValue(RewardsProperty); }
            set { SetValue(RewardsProperty, value); }
        }
        public static readonly DependencyProperty RewardsProperty = DependencyProperty.Register(nameof(Rewards), typeof(RewardBLSVCollection), typeof(RewardCollectionBLSVListViewUserControl));

        public RewardCollectionBLSVListViewUserControl()
        {
            InitializeComponent();
        }
    }
}
