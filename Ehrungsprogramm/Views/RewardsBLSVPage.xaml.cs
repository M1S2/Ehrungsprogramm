using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Ehrungsprogramm.Helpers;
using Ehrungsprogramm.ViewModels;
using MahApps.Metro.Controls;

namespace Ehrungsprogramm.Views
{
    public partial class RewardsBLSVPage : Page
    {
        public RewardsBLSVPage(RewardsBLSVViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewExtensions.GridViewColumnHeaderClickedHandler(sender, e);
        }
    }
}
