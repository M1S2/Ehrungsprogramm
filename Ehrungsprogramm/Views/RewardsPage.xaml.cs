using System.Windows.Controls;

using Ehrungsprogramm.ViewModels;

namespace Ehrungsprogramm.Views
{
    public partial class RewardsPage : Page
    {
        public RewardsPage(RewardsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
