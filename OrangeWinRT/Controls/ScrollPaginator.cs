using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

using Windows.UI.Xaml.Media.Animation;

using OrangeWinRT.Contrib.WinRTXamlToolkit.Controls.Extensions; // for ScrollViewerExtensions

namespace OrangeWinRT.Controls
{
    [TemplatePart(Name = PREV_BUTTON_PARTNAME, Type = typeof(Button))]
    [TemplatePart(Name = NEXT_BUTTON_PARTNAME, Type = typeof(Button))]
    [TemplatePart(Name = SLIDER_PARTNAME, Type = typeof(Slider))]
    public sealed class ScrollPaginator : Control
    {
        const double PAGE_SEPARATION_ALLOWANCE = 0.05;
        const double SIGNIFICANT_DIGITS = 1000;

        #region Template Parts
        private Button _prevButton;
        private Button _nextButton;
        private Slider _slider;

        const string PREV_BUTTON_PARTNAME = "PrevButton";
        const string NEXT_BUTTON_PARTNAME = "NextButton";
        const string SLIDER_PARTNAME = "Slider";
        #endregion

        public ScrollPaginator()
        {
            this.DefaultStyleKey = typeof(ScrollPaginator);
        }

        protected override void OnApplyTemplate()
        {
            this._prevButton = GetTemplateChild(PREV_BUTTON_PARTNAME) as Button;

            if (this._prevButton != null)
            {
                this._prevButton.Click += (_s, _e) =>
                {
                    if (this._scrollViewer == null) { return; }
                    int newPageIndex = this.PageIndex - 1;
                    if (newPageIndex > 0){
                        this.ScrollToPageIndex(newPageIndex, isAnimation: true);
                    }
                };
            }

            this._nextButton = GetTemplateChild(NEXT_BUTTON_PARTNAME) as Button;
            if (this._nextButton != null)
            {
                this._nextButton.Click += (_s, _e) =>
                {
                    if (this._scrollViewer == null) { return; }
                    int newPageIndex = this.PageIndex + 1;
                    if (newPageIndex <= this.NumPages)
                    {
                        this.ScrollToPageIndex(newPageIndex, isAnimation: true);
                    }
                };
            }

            this._slider = GetTemplateChild(SLIDER_PARTNAME) as Slider;
            if (this._slider != null)
            {
                this._slider.ValueChanged += (_s, _e) =>
                {
                    if (this._slider == null || (
                            this.NumPages == this._slider.Maximum &&
                            this.PageIndex == this._slider.Value))
                    {
                        return;
                    }
                    this.ScrollToPageIndex((int)this._slider.Value, isAnimation:false);
                };
            }
            
            base.OnApplyTemplate();
        }

        public DependencyObject Scroller
        {
            get { return (ScrollViewer)GetValue(ScrollerProperty); }
            set { SetValue(ScrollerProperty, value); }
        }

        public static readonly DependencyProperty ScrollerProperty =
            DependencyProperty.Register("Scroller",
                        typeof(DependencyObject),
                        typeof(ScrollPaginator),
                        new PropertyMetadata(null, (depobj, args) =>
                        {
                            ScrollPaginator indicator = (ScrollPaginator)depobj;
                            DependencyObject scroller = (DependencyObject)args.NewValue;

                            ScrollViewer scrollViewer = scroller as ScrollViewer;
                            if (scrollViewer == null)
                            {
                                if (VisualTreeHelper.GetChildrenCount(scroller) == 0) return;
                                var child = VisualTreeHelper.GetChild(scroller, 0);
                                scrollViewer = child as ScrollViewer;
                                if (scrollViewer == null)
                                {
                                    if (VisualTreeHelper.GetChildrenCount(scroller) == 0) return;
                                    scrollViewer = VisualTreeHelper.GetChild(child, 0) as ScrollViewer;
                                    if (scrollViewer == null) return;
                                }
                            }
                            
                            indicator._scrollViewer = scrollViewer;

                            indicator.resetLayoutMetrics();

                            scrollViewer.SizeChanged += (s, e) =>
                            {
                                indicator.resetLayoutMetrics();
                                indicator.OnScrollViewerUpdated();
                            };

                            scrollViewer.LayoutUpdated += (s, e) =>
                            {
                                indicator.OnScrollViewerUpdated();
                            };

                            indicator.OnScrollViewerUpdated();
                        }));

        private ScrollViewer _scrollViewer;
        public ScrollViewer ScrollViewer
        {
            get { return this._scrollViewer; }
        }

        #region NumPages
        public int NumPages
        {
            get { return (int)GetValue(NumPagesProperty); }
            set { SetValue(NumPagesProperty, value); }
        }

        public static readonly DependencyProperty NumPagesProperty = DependencyProperty.Register("NumPages",
            typeof(int), typeof(ScrollPaginator), null);
        #endregion

        #region PageIndex
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex",
            typeof(int), typeof(ScrollPaginator), null);
        #endregion

        private double _lastExtentSize;
        private double _lastOffset = -1;
        private double _startOffset = 0; // need for grid view?
        private double _size = -1;
        private double _extentSize;
        private double _offset;

        private Orientation _orientation;

        public void resetLayoutMetrics()
        {
            this._lastOffset = -1;
            this._startOffset = 0;
            this._size = -1;
        }

        private void _updateLayoutMetrics()
        {
            if (this._scrollViewer.HorizontalScrollMode == ScrollMode.Disabled)
            {
                this._orientation = Orientation.Vertical;
            }
            else
            {
                this._orientation = Orientation.Horizontal;
            }

            if (this._orientation == Orientation.Horizontal)
            {
                if (this._size < 0)
                {
                    this._size = this._scrollViewer.ViewportWidth;
                }
                this._extentSize = this._scrollViewer.ExtentWidth;
                this._offset = this._scrollViewer.HorizontalOffset;
            }
            else
            {
                if (this._size < 0)
                {
                    this._size = this._scrollViewer.ViewportHeight;
                }
                this._extentSize = this._scrollViewer.ExtentHeight;
                this._offset = this._scrollViewer.VerticalOffset;
            }

            if (this._lastOffset < 0)
            {
                this._startOffset = this._offset;
            }
        }

        public void OnScrollViewerUpdated()
        {
            this._updateLayoutMetrics();
            double size = this._size;
            if (size < 0) { return; }

            double extentSize = this._extentSize;
            double offset = this._offset;

            if (extentSize == this._lastExtentSize && offset == this._lastOffset)
            {
                return;
            }
            this._lastExtentSize = extentSize;
            this._lastOffset = offset;

            if (this._startOffset > 0)
            {
                offset -= this._startOffset;
                extentSize -= this._startOffset;
            }

            int numPages = (int)Math.Ceiling((int)(SIGNIFICANT_DIGITS * extentSize / size) / SIGNIFICANT_DIGITS);
            if (extentSize - numPages * size > PAGE_SEPARATION_ALLOWANCE * size)
            {
                this.NumPages = numPages + 1;
            }
            else
            {
                this.NumPages = numPages;
            }

            this.PageIndex = (int)Math.Ceiling((int)(SIGNIFICANT_DIGITS * (offset + size) / size) / SIGNIFICANT_DIGITS);

            if (this._prevButton != null && this._nextButton != null)
            {
                if (this.PageIndex <= 1)
                {
                    this._prevButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._prevButton.Visibility = Visibility.Visible;
                }

                if (this.PageIndex >= this.NumPages)
                {
                    this._nextButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._nextButton.Visibility = Visibility.Visible;
                }
            }

            if (this._slider != null)
            {
               
                if (this.NumPages <= 1)
                {
                    this._slider.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._slider.Visibility = Visibility.Visible;
                    this._slider.Maximum = this.NumPages;
                    this._slider.Minimum = 1;
                    this._slider.Value = this.PageIndex;
                }
            }
        }

        public async void ScrollToPageIndex(int pageIndex, bool isAnimation = false)
        {
            double newOffset = (pageIndex - 1) * this._size + this._startOffset;
            if (this._orientation == Orientation.Horizontal)
            {
                if (isAnimation)
                {
                    await this._scrollViewer.ScrollToHorizontalOffsetWithAnimation(newOffset, 0.5);
                }
                else
                {
                    this._scrollViewer.ScrollToHorizontalOffset(newOffset);
                }
            }
            else
            {
                if (isAnimation)
                {
                    await this._scrollViewer.ScrollToVerticalOffsetWithAnimation(newOffset, 0.5);
                }
                else
                {
                    this._scrollViewer.ScrollToVerticalOffset(newOffset);
                }
            }
        }

        private async void _scrollToOffset(double offset)
        {
            if (this._orientation == Orientation.Horizontal)
            {
                await this._scrollViewer.ScrollToHorizontalOffsetWithAnimation(offset, 0.5);
            }
            else
            {
                await this._scrollViewer.ScrollToVerticalOffsetWithAnimation(offset, 0.5);
            }
        }
    }
}

