using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;

namespace OrangeWinRT.SampleApp.Common
{
    [Windows.UI.Xaml.Markup.ContentProperty(Name = "RichTextContent")]
    public sealed class RichTextColumns : Panel
    {
        public static readonly DependencyProperty RichTextContentProperty =
            DependencyProperty.Register("RichTextContent", typeof(RichTextBlock),
            typeof(RichTextColumns), new PropertyMetadata(null, ResetOverflowLayout));

        public static readonly DependencyProperty ColumnTemplateProperty =
            DependencyProperty.Register("ColumnTemplate", typeof(DataTemplate),
            typeof(RichTextColumns), new PropertyMetadata(null, ResetOverflowLayout));

        public RichTextColumns()
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
        }

        public RichTextBlock RichTextContent
        {
            get { return (RichTextBlock)GetValue(RichTextContentProperty); }
            set { SetValue(RichTextContentProperty, value); }
        }

        public DataTemplate ColumnTemplate
        {
            get { return (DataTemplate)GetValue(ColumnTemplateProperty); }
            set { SetValue(ColumnTemplateProperty, value); }
        }

        private static void ResetOverflowLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as RichTextColumns;
            if (target != null)
            {
                target._overflowColumns = null;
                target.Children.Clear();
                target.InvalidateMeasure();
            }
        }

        private List<RichTextBlockOverflow> _overflowColumns = null;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.RichTextContent == null) return new Size(0, 0);

            if (this._overflowColumns == null)
            {
                Children.Add(this.RichTextContent);
                this._overflowColumns = new List<RichTextBlockOverflow>();
            }

            this.RichTextContent.Measure(availableSize);
            var maxWidth = this.RichTextContent.DesiredSize.Width;
            var maxHeight = this.RichTextContent.DesiredSize.Height;
            var hasOverflow = this.RichTextContent.HasOverflowContent;

            int overflowIndex = 0;
            while (hasOverflow && maxWidth < availableSize.Width && this.ColumnTemplate != null)
            {
                RichTextBlockOverflow overflow;
                if (this._overflowColumns.Count > overflowIndex)
                {
                    overflow = this._overflowColumns[overflowIndex];
                }
                else
                {
                    overflow = (RichTextBlockOverflow)this.ColumnTemplate.LoadContent();
                    this._overflowColumns.Add(overflow);
                    this.Children.Add(overflow);
                    if (overflowIndex == 0)
                    {
                        this.RichTextContent.OverflowContentTarget = overflow;
                    }
                    else
                    {
                        this._overflowColumns[overflowIndex - 1].OverflowContentTarget = overflow;
                    }
                }

                overflow.Measure(new Size(availableSize.Width - maxWidth, availableSize.Height));
                maxWidth += overflow.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, overflow.DesiredSize.Height);
                hasOverflow = overflow.HasOverflowContent;
                overflowIndex++;
            }

            if (this._overflowColumns.Count > overflowIndex)
            {
                if (overflowIndex == 0)
                {
                    this.RichTextContent.OverflowContentTarget = null;
                }
                else
                {
                    this._overflowColumns[overflowIndex - 1].OverflowContentTarget = null;
                }
                while (this._overflowColumns.Count > overflowIndex)
                {
                    this._overflowColumns.RemoveAt(overflowIndex);
                    this.Children.RemoveAt(overflowIndex + 1);
                }
            }

            return new Size(maxWidth, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double maxWidth = 0;
            double maxHeight = 0;
            foreach (var child in Children)
            {
                child.Arrange(new Rect(maxWidth, 0, child.DesiredSize.Width, finalSize.Height));
                maxWidth += child.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }
            return new Size(maxWidth, maxHeight);
        }
    }
}
