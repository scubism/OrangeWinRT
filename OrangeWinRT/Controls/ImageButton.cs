using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

using Windows.UI.Xaml.Media.Imaging;

namespace OrangeWinRT.Controls
{
    public sealed class ImageButton : Button
    {
        TextBlock _textBlock;
        Image _image;

        public ImageButton()
        {
            this.DefaultStyleKey = typeof(ImageButton);

            var grid = new Grid();

            this._image = new Image();
            grid.Children.Add(this._image);

            this._textBlock = new TextBlock();
            this._textBlock.VerticalAlignment = VerticalAlignment.Center;
            this._textBlock.TextAlignment = TextAlignment.Center;
            grid.Children.Add(this._textBlock);

            this.Content = grid;
        }

        #region Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string), typeof(ImageButton), 
            new PropertyMetadata(null, (depobj, args) =>
                {
                    ((ImageButton)depobj)._textBlock.Text = (string)args.NewValue;
                }));
        #endregion

        #region ImageSource
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource",
            typeof(ImageSource), typeof(ImageButton), 
            new PropertyMetadata(null, (depobj, args) =>
                {
                    ((ImageButton)depobj)._image.Source = (ImageSource)args.NewValue;
                }));
        #endregion

        #region ImageWidth
        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth",
            typeof(double), typeof(ImageButton), 
            new PropertyMetadata(null, (depobj, args) =>
                {
                    ((ImageButton)depobj)._image.Width = (double)args.NewValue;
                }));
        #endregion

        #region ImageHeight
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight",
            typeof(double), typeof(ImageButton), 
            new PropertyMetadata(null, (depobj, args) =>
                {
                    ((ImageButton)depobj)._image.Height = (double)args.NewValue;
                }));
        #endregion
    }
}
