using System.Windows;
using System.Windows.Controls;
using Ehrungsprogramm.Helpers;
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

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewExtensions.GridViewColumnHeaderClickedHandler(sender, e);
        }
    }
}
