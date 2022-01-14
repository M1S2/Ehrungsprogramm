using System.Windows.Controls;

using Ehrungsprogramm.ViewModels;

namespace Ehrungsprogramm.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
