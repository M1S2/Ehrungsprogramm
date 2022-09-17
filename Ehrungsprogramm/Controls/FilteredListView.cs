using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Toolkit.Mvvm.Input;
using System.Threading;
using System.Linq;

namespace Ehrungsprogramm.Controls
{
    /// <summary>
    /// Führen Sie die Schritte 1a oder 1b und anschließend Schritt 2 aus, um dieses benutzerdefinierte Steuerelement in einer XAML-Datei zu verwenden.
    ///
    /// Schritt 1a) Verwenden des benutzerdefinierten Steuerelements in einer XAML-Datei, die im aktuellen Projekt vorhanden ist.
    /// Fügen Sie dieses XmlNamespace-Attribut dem Stammelement der Markupdatei 
    /// an der Stelle hinzu, an der es verwendet werden soll:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Ehrungsprogramm.Controls"
    ///
    ///
    /// Schritt 1b) Verwenden des benutzerdefinierten Steuerelements in einer XAML-Datei, die in einem anderen Projekt vorhanden ist.
    /// Fügen Sie dieses XmlNamespace-Attribut dem Stammelement der Markupdatei 
    /// an der Stelle hinzu, an der es verwendet werden soll:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Ehrungsprogramm.Controls;assembly=Ehrungsprogramm.Controls"
    ///
    /// Darüber hinaus müssen Sie von dem Projekt, das die XAML-Datei enthält, einen Projektverweis
    /// zu diesem Projekt hinzufügen und das Projekt neu erstellen, um Kompilierungsfehler zu vermeiden:
    ///
    ///     Klicken Sie im Projektmappen-Explorer mit der rechten Maustaste auf das Zielprojekt und anschließend auf
    ///     "Verweis hinzufügen"->"Projekte"->[Navigieren Sie zu diesem Projekt, und wählen Sie es aus.]
    ///
    ///
    /// Schritt 2)
    /// Fahren Sie fort, und verwenden Sie das Steuerelement in der XAML-Datei.
    ///
    ///     <MyNamespace:FilteredListView/>
    ///
    /// </summary>
    /// <see cref="https://www.codeproject.com/Articles/1268558/A-WPF-ListView-Custom-Control-with-Search-Filter-T"/>
    public class FilteredListView : ListView
    {
        static FilteredListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilteredListView), new FrameworkPropertyMetadata(typeof(FilteredListView)));
        }

        public FilteredListView()
        {
            SetDefaultFilterPredicate();
            InitThrottle();
        }

        public Subject<bool> FilterInputSubject = new Subject<bool>();

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Function used to decide, which elements of the list are shown
        /// object: list element
        /// string: filter text
        /// bool: return true here to show the element; otherwise false
        /// </summary>
        public Func<object, string, bool> FilterPredicate
        {
            get { return (Func<object, string, bool>)GetValue(FilterPredicateProperty); }
            set { SetValue(FilterPredicateProperty, value); }
        }
        public static readonly DependencyProperty FilterPredicateProperty = DependencyProperty.Register(nameof(FilterPredicate), typeof(Func<object, string, bool>), typeof(FilteredListView), new PropertyMetadata(null));

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Text that is used for filtering the list
        /// </summary>
        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }
        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(nameof(FilterText), typeof(string), typeof(FilteredListView),
                new PropertyMetadata("", (d, e) => (d as FilteredListView).FilterInputSubject.OnNext(true)));  //This is the 'PropertyChanged' callback that's called whenever the Filter input text is changed

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private ICommand _clearFilterCommand;
        public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new RelayCommand(() => FilterText = ""));

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void SetDefaultFilterPredicate()
        {
            FilterPredicate = (obj, text) => obj.ToString().ToLower().Contains(text);
        }

        private void InitThrottle()
        {
            FilterInputSubject.Throttle(TimeSpan.FromMilliseconds(250)).ObserveOn(SynchronizationContext.Current).Subscribe(HandleFilterThrottle);
        }

        private void HandleFilterThrottle(bool b)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.ItemsSource);
            if (collectionView == null) return;
            collectionView.Filter = (item) => FilterPredicate(item, FilterText);
        }

    }
}
