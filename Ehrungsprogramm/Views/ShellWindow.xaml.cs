using System.Windows.Controls;

using Ehrungsprogramm.Contracts.Views;
using Ehrungsprogramm.ViewModels;

using MahApps.Metro.Controls;

namespace Ehrungsprogramm.Views
{
    public partial class ShellWindow : MetroWindow, IShellWindow
    {
        public ShellWindow(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public Frame GetNavigationFrame()
            => shellFrame;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();
    }
}
