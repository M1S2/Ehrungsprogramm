using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Ehrungsprogramm.ViewModels;
using MahApps.Metro.Controls;

namespace Ehrungsprogramm.Views
{
    public partial class RewardsPage : Page
    {
        public RewardsPage(RewardsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // Sort the column when clicked on the header
        // https://learn.microsoft.com/de-de/dotnet/desktop/wpf/controls/how-to-sort-a-gridview-column-when-a-header-is-clicked?view=netframeworkdesktop-4.8

        Dictionary<ListView, GridViewColumnHeader> _lastHeaderClicked = new Dictionary<ListView, GridViewColumnHeader>();
        Dictionary<ListView, ListSortDirection> _lastDirection = new Dictionary<ListView, ListSortDirection>(); 

        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListView lv = headerClicked.TryFindParent<ListView>();
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (!_lastHeaderClicked.ContainsKey(lv)) { _lastHeaderClicked.Add(lv, headerClicked); }
                    if (headerClicked != _lastHeaderClicked[lv])
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (!_lastDirection.ContainsKey(lv)) { _lastDirection.Add(lv, ListSortDirection.Ascending); }
                        if (_lastDirection[lv] == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    Binding columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    string sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        ICollectionView dataView = CollectionViewSource.GetDefaultView(lv.ItemsSource);
                        dataView.SortDescriptions.Clear();
                        SortDescription sd = new SortDescription(sortBy, direction);
                        dataView.SortDescriptions.Add(sd);
                        dataView.Refresh();

                        if (direction == ListSortDirection.Ascending)
                        {
                            headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
                        }
                        else
                        {
                            headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
                        }

                        // Remove arrow from previously sorted header
                        if (_lastHeaderClicked != null && _lastHeaderClicked[lv] != headerClicked)
                        {
                            _lastHeaderClicked[lv].Column.HeaderTemplate = null;
                        }

                        if (_lastHeaderClicked.ContainsKey(lv))
                        {
                            _lastHeaderClicked[lv] = headerClicked;
                        }
                        else
                        {
                            _lastHeaderClicked.Add(lv, headerClicked);
                        }

                        if (_lastDirection.ContainsKey(lv))
                        {
                            _lastDirection[lv] = direction;
                        }
                        else
                        {
                            _lastDirection.Add(lv, direction);
                        }
                    }
                }
            }
        }

    }
}
