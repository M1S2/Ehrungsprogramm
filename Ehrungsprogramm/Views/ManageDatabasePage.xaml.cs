using System.Windows.Controls;

using Ehrungsprogramm.ViewModels;

namespace Ehrungsprogramm.Views
{
    public partial class ManageDatabasePage : Page
    {
        public ManageDatabasePage(ManageDatabaseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
