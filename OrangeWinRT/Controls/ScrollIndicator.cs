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
    public sealed class ScrollIndicator : Control
    {
        #region Template Parts
        private Button _prevButton;
        private Button _nextButton;

        const string PREV_BUTTON_PARTNAME = "PrevButton";
        const string NEXT_BUTTON_PARTNAME = "NextButton";
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
                    this.ScrollToPrev();
                };
            }

            this._nextButton = GetTemplateChild("NextButton") as Button;
            if (this._nextButton != null)
            {
                this._nextButton.Click += (_s, _e) =>
                {
                    if (this.ScrollViewer == null) { return; }
                    this.ScrollToNext();
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

                            scrollViewer.LayoutUpdated += (s, e) =>
                            {
                                indicator.OnScrollViewerUpdated();
                            };
                            indicator.OnScrollViewerUpdated();
                        }));
        #endregion

        #region Slider
        public Slider Slider
        {
            get { return (Slider)GetValue(SliderProperty); }
            set { SetValue(SliderProperty, value); }
        }

        public static DependencyProperty SliderProperty =
            DependencyProperty.Register("Slider",
                        typeof(Slider),
                        typeof(ScrollIndicator),
                        new PropertyMetadata(null, OnSliderPropertyChanged));

        private static void OnSliderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = (ScrollIndicator)d;
            indicator.Slider.ValueChanged += (_s, _e) =>
            {
                if (indicator.ScrollViewer == null || (
                        indicator.NumPages == indicator.Slider.Maximum &&
                        indicator.PageIndex == indicator.Slider.Value))
                {
                    return;
                }
                indicator.ScrollToPageIndex((int)indicator.Slider.Value);
            };
        }
        #endregion

        #region TextBlock
        public TextBlock TextBlock
        {
            get { return (TextBlock)GetValue(TextBlockProperty); }
            set { SetValue(TextBlockProperty, value); }
        }

        public static DependencyProperty TextBlockProperty =
            DependencyProperty.Register("TextBlock",
                        typeof(TextBlock),
                        typeof(ScrollIndicator),
                        new PropertyMetadata(null, null));
        #endregion

        public int NumPages;
        public int PageIndex;

        private double _lastSize;
        private double _lastExtentSize;
        private double _lastOffset = -1;
        private double _startOffset = 0; // need for grid view?
        private double _size = -1;
        private double _extentSize;
        private double _offset;

        private Orientation _orientation;

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
                this._size = this.ScrollViewer.ViewportWidth;
                this._extentSize = this.ScrollViewer.ExtentWidth;
                this._offset = this.ScrollViewer.HorizontalOffset;
            }
            else
            {
                this._size = this.ScrollViewer.ViewportHeight;
                this._extentSize = this.ScrollViewer.ExtentHeight;
                this._offset = this.ScrollViewer.VerticalOffset;
            }
        }

        public void OnScrollViewerUpdated()
        {
            this._updateLayoutMetrics();
            double size = this._size;
            double extentSize = this._extentSize;
            double offset = this._offset;

            if (size == this._lastSize && extentSize == this._lastExtentSize && offset == this._lastOffset)
            {
                return;
            }
            this._lastSize = size;
            this._lastExtentSize = extentSize;

            if (this._lastOffset < 0)
            {
                this._startOffset = offset;
            }
            this._lastOffset = offset;
            if (this._startOffset > 0)
            {
                offset -= this._startOffset;
                extentSize -= this._startOffset;
            }

            int numPages = (int)Math.Round(extentSize / size);
            if (extentSize - numPages * size > 0.1 * size)
            {
                numPages += 1;
            }
            this.NumPages = numPages;

            int pageIndex = (int)Math.Round((offset + size) / size);
            if (offset + size - numPages * size > 0.1 * size)
            {
                pageIndex += 1;
            }
            this.PageIndex = pageIndex;

            
            if (this._size <= 0) { return; }
            if (this._startOffset > 0)
            {
                offset -= this._startOffset;
                extentSize -= this._startOffset;
            }

            if (this._prevButton != null && this._nextButton != null)
            {
                if (offset < 0.1 * size)
                {
                    this._prevButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._prevButton.Visibility = Visibility.Visible;
                }

                if (offset > extentSize - 1.1 * size)
                {
                    this._nextButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._nextButton.Visibility = Visibility.Visible;
                }
            }

            if (this.Slider != null)
            {
                if (numPages <= 1)
                {
                    this.Slider.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.Slider.Visibility = Visibility.Visible;
                    this.Slider.Maximum = this.NumPages;
                    this.Slider.Minimum = 1;
                    this.Slider.Value = this.PageIndex;
                }
            }

            if (this.TextBlock != null)
            {
                if (this.NumPages <= 1)
                {
                    this.TextBlock.Text = "";
                }
                else
                {
                    // TODO enable format setting
                    this.TextBlock.Text = String.Format("{0}/{1}", this.PageIndex, this.NumPages);
                }
            }
        }

        public void ScrollToPageIndex(int pageIndex)
        {
            double newOffset = (pageIndex - 1) * this._size + this._startOffset;
            if (this._orientation == Orientation.Horizontal)
            {
                this.ScrollViewer.ScrollToHorizontalOffset(newOffset);
            }
            else
            {
                this.ScrollViewer.ScrollToVerticalOffset(newOffset);
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

        public void ScrollToPrev()
        {
            this._updateLayoutMetrics();
            double newOffset = this._offset - this._size;
            if (newOffset < this._startOffset)
            {
                newOffset = this._startOffset;
            }
            // ! Extend ScrollViewer to implemnt ScrollToHorizontalOffsetWithAnimation method
            this._scrollToOffset(newOffset);
        }

        public void ScrollToNext()
        {
            this._updateLayoutMetrics();
            double newOffset = this._offset + this._size;
            if (newOffset > this._extentSize - this._size &&
                    this._extentSize > this._size)
            {
                newOffset = this._extentSize - this._size;
            }
            // ! Extend ScrollViewer to implemnt ScrollToHorizontalOffsetWithAnimation method
            this._scrollToOffset(newOffset);
        }

    }
}

