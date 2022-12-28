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
    /// <summary>
    /// This class contains a static method <see cref="GridViewColumnHeaderClickedHandler(object, RoutedEventArgs)"/> that can be used to support sorting for ListViews by header clicks.
    /// 
    /// The sort descriptions are created by using the DisplayMemberBinding of the GridViewColumn. 
    /// If a column doesn't use the DisplayMemberBinding (because e.g. a more complicated CellTemplate is used), set the Tag of the GridViewColumnHeader to the sort property with the following snippet:
    ///
    /// Example:
    /// 
    /// <ListView ... GridViewColumnHeader.Click="GridViewColumnHeader_Click">
    ///     <ListView.View>
    ///         <GridView>
    ///             <GridViewColumn Header="ColumnHeader" DisplayMemberBinding="{Binding Property}">
    ///             <GridViewColumn>
    ///                 <GridViewColumn.Header>
    ///                     <GridViewColumnHeader Tag = "SortPropertyName">
    ///                         ...
    ///                     </GridViewColumnHeader>
    ///                 </GridViewColumn.Header>
    ///                 <GridViewColumn.CellTemplate>
    ///                     ...
    ///                 </GridViewColumn.CellTemplate>
    ///             </GridViewColumn>
    ///         </GridView>
    ///     </ListView.View>
    /// </ListView>
    ///
    /// </summary>
    public static class GridViewExtensions
    {
        private static Dictionary<ListView, GridViewColumnHeader> _lastHeaderClicked = new Dictionary<ListView, GridViewColumnHeader>();
        private static Dictionary<ListView, ListSortDirection> _lastDirection = new Dictionary<ListView, ListSortDirection>();
        private static Dictionary<ListView, string> _lastSortedBy = new Dictionary<ListView, string>();

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

                    if (!_lastSortedBy.ContainsKey(lv)) { _lastSortedBy.Add(lv, "Name"); }

                    Binding columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    string sortBy = (columnBinding?.Path.Path ?? headerClicked.Column.Header as string ?? headerClicked.Tag) as string;

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        ICollectionView dataView = CollectionViewSource.GetDefaultView(lv.ItemsSource);

                        string lastSortBy = _lastSortedBy[lv];
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

                        _lastSortedBy[lv] = sortBy;
                    }
                }
            }
        }

    }
}
