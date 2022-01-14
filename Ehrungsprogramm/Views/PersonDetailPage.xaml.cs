using System.Windows.Controls;

using Ehrungsprogramm.ViewModels;

namespace Ehrungsprogramm.Views
{
    public partial class PersonDetailPage : Page
    {
        public PersonDetailPage(PersonDetailViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
