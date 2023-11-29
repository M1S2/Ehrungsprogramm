using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;
using System.ComponentModel;

namespace Ehrungsprogramm.Helpers
{
    /// <summary>
    /// Adorner used to indicate the sorting direction of a list (ascending or descending)
    /// </summary>
    /// see: https://wpf-tutorial.com/listview-control/listview-how-to-column-sorting/
    public class SortAdorner : Adorner
    {
        private static Geometry ascGeometry = Geometry.Parse("M 0 7 L 5 0 L 10 7 Z");
        private static Geometry descGeometry = Geometry.Parse("M 0 0 L 5 7 L 10 0 Z");

        public ListSortDirection Direction { get; private set; }

        public Brush DrawingBrush { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir, Brush drawingBrush = null)
            : base(element)
        {
            this.Direction = dir;
            DrawingBrush = drawingBrush ?? Brushes.Black;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            TranslateTransform transform = new TranslateTransform
                (
                    AdornedElement.RenderSize.Width - 15,
                    (AdornedElement.RenderSize.Height - 5) / 2
                );
            drawingContext.PushTransform(transform);

            Geometry geometry = ascGeometry;
            if (this.Direction == ListSortDirection.Descending)
                geometry = descGeometry;
            drawingContext.DrawGeometry(DrawingBrush, null, geometry);

            drawingContext.Pop();
        }
    }
}
