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
    /// Interaktionslogik für RewardDetailUserControl.xaml
    /// </summary>
    public partial class RewardDetailUserControl : UserControl
    {
        /// <summary>
        /// Reward for which the detail view is generated
        /// </summary>
        public Reward Reward
        {
            get { return (Reward)GetValue(RewardProperty); }
            set { SetValue(RewardProperty, value); }
        }
        public static readonly DependencyProperty RewardProperty = DependencyProperty.Register(nameof(Reward), typeof(Reward), typeof(RewardDetailUserControl));

        /// <summary>
        /// Constructor of the RewardDetailUserControl
        /// </summary>
        public RewardDetailUserControl()
        {
            InitializeComponent();
        }
    }
}
