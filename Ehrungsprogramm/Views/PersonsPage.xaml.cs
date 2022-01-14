using System.Windows.Controls;

using Ehrungsprogramm.ViewModels;

namespace Ehrungsprogramm.Views
{
    public partial class PersonsPage : Page
    {
        public PersonsPage(PersonsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
