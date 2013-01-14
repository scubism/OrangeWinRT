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

using WinRTXamlToolkit.Controls.Extensions; // for ScrollViewerExtensions

namespace OrangeWinRT.Controls
{
    [TemplatePart(Name = PREV_BUTTON_PARTNAME, Type = typeof(Button))]
    [TemplatePart(Name = NEXT_BUTTON_PARTNAME, Type = typeof(Button))]
    [TemplatePart(Name = SLIDER_PARTNAME, Type = typeof(Slider))]
    public sealed class ScrollIndicator : Control
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

        public ScrollIndicator()
        {
            this.DefaultStyleKey = typeof(ScrollIndicator);
        }

        protected override void OnApplyTemplate()
        {
            this._prevButton = GetTemplateChild("PrevButton") as Button;

            if (this._prevButton != null)
            {
                this._prevButton.Click += (_s, _e) =>
                {
                    if (this.ScrollViewer == null) { return; }
                    int newPageIndex = this.PageIndex - 1;
                    if (newPageIndex > 0){
                        this.ScrollToPageIndex(newPageIndex, isAnimation: true);
                    }
                };
            }

            this._nextButton = GetTemplateChild("NextButton") as Button;
            if (this._nextButton != null)
            {
                this._nextButton.Click += (_s, _e) =>
                {
                    if (this.ScrollViewer == null) { return; }
                    int newPageIndex = this.PageIndex + 1;
                    if (newPageIndex <= this.NumPages)
                    {
                        this.ScrollToPageIndex(newPageIndex, isAnimation: true);
                    }
                };
            }

            this._slider = GetTemplateChild("Slider") as Slider;
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

        #region ScrollViewer
        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer",
                        typeof(ScrollViewer),
                        typeof(ScrollIndicator),
                        new PropertyMetadata(null, (depobj, args) =>
                        {
                            ScrollIndicator indicator = (ScrollIndicator)depobj;
                            ScrollViewer scrollViewer = (ScrollViewer)args.NewValue;

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
        #endregion

        #region NumPages
        public int NumPages
        {
            get { return (int)GetValue(NumPagesProperty); }
            set { SetValue(NumPagesProperty, value); }
        }

        public static readonly DependencyProperty NumPagesProperty = DependencyProperty.Register("NumPages",
            typeof(int),
            typeof(ScrollIndicator),
            new PropertyMetadata(null, null));
        #endregion

        #region PageIndex
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex",
            typeof(int),
            typeof(ScrollIndicator),
            new PropertyMetadata(null, null));
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
            if (this.ScrollViewer.HorizontalScrollMode == ScrollMode.Disabled)
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
                    this._size = this.ScrollViewer.ViewportWidth;
                }
                this._extentSize = this.ScrollViewer.ExtentWidth;
                this._offset = this.ScrollViewer.HorizontalOffset;
            }
            else
            {
                if (this._size < 0)
                {
                    this._size = this.ScrollViewer.ViewportHeight;
                }
                this._extentSize = this.ScrollViewer.ExtentHeight;
                this._offset = this.ScrollViewer.VerticalOffset;
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
                    await this.ScrollViewer.ScrollToHorizontalOffsetWithAnimation(newOffset, 0.5);
                }
                else
                {
                    this.ScrollViewer.ScrollToHorizontalOffset(newOffset);
                }
            }
            else
            {
                if (isAnimation)
                {
                    await this.ScrollViewer.ScrollToVerticalOffsetWithAnimation(newOffset, 0.5);
                }
                else
                {
                    this.ScrollViewer.ScrollToVerticalOffset(newOffset);
                }
            }
        }

        private async void _scrollToOffset(double offset)
        {
            if (this._orientation == Orientation.Horizontal)
            {
                await this.ScrollViewer.ScrollToHorizontalOffsetWithAnimation(offset, 0.5);
            }
            else
            {
                await this.ScrollViewer.ScrollToVerticalOffsetWithAnimation(offset, 0.5);
            }
        }
    }
}

