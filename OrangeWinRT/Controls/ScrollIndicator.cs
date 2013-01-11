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
    public sealed class ScrollIndicator : Control
    {
        public ScrollIndicator()
        {
            this.DefaultStyleKey = typeof(ScrollIndicator);
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
            indicator.UpdateElements();
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

        #region PrevButton
        public Button PrevButton
        {
            get { return (Button)GetValue(PrevButtonProperty); }
            set { SetValue(PrevButtonProperty, value); }
        }

        public static DependencyProperty PrevButtonProperty =
            DependencyProperty.Register("PrevButton",
                        typeof(Button),
                        typeof(ScrollIndicator),
                        new PropertyMetadata(null, OnPrevButtonPropertyChanged));

        private static void OnPrevButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = (ScrollIndicator)d;

            indicator.PrevButton.Click += (_s, _e) =>
            {
                if (indicator.ScrollViewer == null) { return; }
                indicator.ScrollToPrev();
            };
            indicator.UpdateElements();
        }
        #endregion

        #region NextButton
        public Button NextButton
        {
            get { return (Button)GetValue(NextButtonProperty); }
            set { SetValue(NextButtonProperty, value); }
        }

        public static DependencyProperty NextButtonProperty =
            DependencyProperty.Register("NextButton",
                        typeof(Button),
                        typeof(ScrollIndicator),
                        new PropertyMetadata(null, OnNextButtonPropertyChanged));

        private static void OnNextButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = (ScrollIndicator)d;

            indicator.NextButton.Click += (_s, _e) =>
            {
                if (indicator.ScrollViewer == null) { return; }
                indicator.ScrollToNext();
            };
            indicator.UpdateElements();
        }
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

        private void _updateLayoutMetrics()
        {
            this._size = this.ScrollViewer.ViewportWidth;
            this._extentSize = this.ScrollViewer.ExtentWidth;
            this._offset = this.ScrollViewer.HorizontalOffset;
        }

        public void OnScrollViewerUpdated()
        {
            if (this.ScrollViewer == null) { return; }

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

            this.UpdateElements();
        }

        public void UpdateElements()
        {
            double size = this._size;
            double extentSize = this._extentSize;
            double offset = this._offset;
            if (this._startOffset > 0)
            {
                offset -= this._startOffset;
                extentSize -= this._startOffset;
            }

            if (size <= 0) { return; }


            int numPages = this.NumPages;
            if (this.PrevButton != null && this.NextButton != null)
            {
                if (offset < 0.1 * size)
                {
                    this.PrevButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.PrevButton.Visibility = Visibility.Visible;
                }

                if (offset > extentSize - 1.1 * size)
                {
                    this.NextButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.NextButton.Visibility = Visibility.Visible;
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
            this.ScrollViewer.ScrollToHorizontalOffset(newOffset);
        }

        private async void _scrollToOffset(double offset)
        {
            await this.ScrollViewer.ScrollToHorizontalOffsetWithAnimation(offset, 0.5);
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

