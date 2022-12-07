using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using MahApps.Metro.Controls;

namespace Ehrungsprogramm.Helpers
{
    public static class GridViewExtensions
    {
        private static Dictionary<ListView, GridViewColumnHeader> _lastHeaderClicked = new Dictionary<ListView, GridViewColumnHeader>();
        private static Dictionary<ListView, ListSortDirection> _lastDirection = new Dictionary<ListView, ListSortDirection>();

        /// <summary>
        /// Sort the corresponding column when clicked on the header
        /// </summary>
        /// https://learn.microsoft.com/de-de/dotnet/desktop/wpf/controls/how-to-sort-a-gridview-column-when-a-header-is-clicked?view=netframeworkdesktop-4.8
        public static void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
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

                        Binding lastColumnBinding = _lastHeaderClicked[lv].Column.DisplayMemberBinding as Binding;
                        string lastSortBy = lastColumnBinding?.Path.Path ?? _lastHeaderClicked[lv].Column.Header as string;
                        dataView.SortDescriptions.Remove(dataView.SortDescriptions.Where(s => s.PropertyName == lastSortBy).FirstOrDefault());
                        SortDescription sd = new SortDescription(sortBy, direction);
                        dataView.SortDescriptions.Add(sd);
                        dataView.Refresh();

                        if (direction == ListSortDirection.Ascending)
                        {
                            headerClicked.Column.HeaderTemplate = App.Current.Resources["GridViewHeaderTemplateArrowUp"] as DataTemplate;
                        }
                        else
                        {
                            headerClicked.Column.HeaderTemplate = App.Current.Resources["GridViewHeaderTemplateArrowDown"] as DataTemplate;
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
