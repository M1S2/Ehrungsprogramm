using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ehrungsprogramm.Controls
{
    /// <summary>
    /// Class that can be used to attach a sticky scroll header behaviour to a scroll control
    /// The attached Class has following key-features:
    /// - MVVM compatible
    /// - no Code-Behind
    /// - compatible with ListView, GridView, ItemsControl, even static xaml! - should work with anything using a ScollViewer...
    /// - Uses attached property to declare the group item
    /// </summary>
    /// <see https://stackoverflow.com/questions/14801102/wpf-scroll-itemcontrol-content-fixed-header/>
    /// <Alternative for sticky group headers: see https://github.com/RSchwoerer/code-examples/blob/master/ListBoxAnimatedGroup/DemoAnimatedGroup/Behaviors/OverlayGroupingBehavior.cs/>
    public static class StickyScrollHeader
    {
        public static FrameworkElement GetAttachToControl(FrameworkElement obj)
        {
            return (FrameworkElement)obj.GetValue(AttachToControlProperty);
        }

        public static void SetAttachToControl(FrameworkElement obj, FrameworkElement value)
        {
            obj.SetValue(AttachToControlProperty, value);
        }

        private static ScrollViewer FindScrollViewer(FrameworkElement item)
        {
            FrameworkElement treeItem = item;
            FrameworkElement directItem = item;

            while (treeItem != null)
            {
                treeItem = VisualTreeHelper.GetParent(treeItem) as FrameworkElement;
                if (treeItem is ScrollViewer)
                {
                    return treeItem as ScrollViewer;
                }
                else if (treeItem is ScrollContentPresenter)
                {
                    return (treeItem as ScrollContentPresenter).ScrollOwner;
                }
            }

            while (directItem != null)
            {
                directItem = directItem.Parent as FrameworkElement;

                if (directItem is ScrollViewer)
                {
                    return directItem as ScrollViewer;
                }
                else if (directItem is ScrollContentPresenter)
                {
                    return (directItem as ScrollContentPresenter).ScrollOwner;
                }
            }

            return null;
        }

        private static ScrollContentPresenter FindScrollContentPresenter(FrameworkElement sv)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(sv);

            for (int i = 0; i < childCount; i++)
            {
                if (VisualTreeHelper.GetChild(sv, i) is FrameworkElement child && child is ScrollContentPresenter)
                {
                    return child as ScrollContentPresenter;
                }
            }

            for (int i = 0; i < childCount; i++)
            {
                if (FindScrollContentPresenter(VisualTreeHelper.GetChild(sv, i) as FrameworkElement) is FrameworkElement child && child is ScrollContentPresenter)
                {
                    return child as ScrollContentPresenter;
                }
            }

            return null;
        }

        public static readonly DependencyProperty AttachToControlProperty =
            DependencyProperty.RegisterAttached("AttachToControl", typeof(FrameworkElement), typeof(StickyScrollHeader), new PropertyMetadata(null, (s, e) =>
            {
                try
                {
                    if (!(s is FrameworkElement targetControl))
                    { return; }

                    Canvas.SetZIndex(targetControl, 999);

                    ScrollViewer sv;
                    FrameworkElement parent;

                    if (e.OldValue is FrameworkElement oldParentControl)
                    {
                        ScrollViewer oldSv = FindScrollViewer(oldParentControl);
                        parent = oldParentControl;
                        oldSv.ScrollChanged -= Sv_ScrollChanged;
                    }

                    if (e.NewValue is FrameworkElement newParentControl)
                    {
                        sv = FindScrollViewer(newParentControl);
                        parent = newParentControl;
                        sv.ScrollChanged += Sv_ScrollChanged;
                    }

                    void Sv_ScrollChanged(object sender, ScrollChangedEventArgs sce)
                    {
                        if (!parent.IsVisible) { return; }

                        try
                        {

                            ScrollViewer isv = sender as ScrollViewer;
                            ScrollContentPresenter scp = FindScrollContentPresenter(isv);

                            var relativeTransform = parent.TransformToAncestor(scp);
                            Rect parentRenderRect = relativeTransform.TransformBounds(new Rect(new Point(0, 0), parent.RenderSize));
                            Rect intersectingRect = Rect.Intersect(new Rect(new Point(0, 0), scp.RenderSize), parentRenderRect);
                            if (intersectingRect != Rect.Empty)
                            {
                                TranslateTransform targetTransform = new TranslateTransform();

                                if (parentRenderRect.Top < 0)
                                {
                                    double tempTop = (parentRenderRect.Top * -1);

                                    if (tempTop + targetControl.RenderSize.Height < parent.RenderSize.Height)
                                    {
                                        targetTransform.Y = tempTop;
                                    }
                                    else if (tempTop < parent.RenderSize.Height)
                                    {
                                        targetTransform.Y = tempTop - (targetControl.RenderSize.Height - intersectingRect.Height);
                                    }
                                }
                                else
                                {
                                    targetTransform.Y = 0;
                                }
                                targetControl.RenderTransform = targetTransform;
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }));
    }
}
