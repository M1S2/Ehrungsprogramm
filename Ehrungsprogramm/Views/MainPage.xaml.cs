using System.Windows.Controls;

using Ehrungsprogramm.ViewModels;

namespace Ehrungsprogramm.Views
{
    public partial class MainPage : Page
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
