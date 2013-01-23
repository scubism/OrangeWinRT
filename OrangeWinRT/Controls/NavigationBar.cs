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

namespace OrangeWinRT.Controls
{

    [TemplatePart(Name = BACK_BUTTON_PARTNAME, Type = typeof(Button))]
    public sealed class NavigationBar : Control
    {
        #region Template Parts
        private Button _backButton;

        const string BACK_BUTTON_PARTNAME = "BackButton";
        #endregion

        public NavigationBar()
        {
            this.DefaultStyleKey = typeof(NavigationBar);
        }

        protected override void OnApplyTemplate()
        {
            this._backButton = GetTemplateChild(BACK_BUTTON_PARTNAME) as Button;
            base.OnApplyTemplate();

            this._backButton.Click += (s, e) =>
            {
                if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
            };
            this._updateBackButton();

        }

        private void _updateBackButton()
        {
            if (this._backButton != null)
            {
                if (this.Frame != null && this.Frame.CanGoBack)
                {
                    this._backButton.Visibility = Visibility.Visible;
                }
                else
                {
                    this._backButton.Visibility = Visibility.Collapsed;
                }

            }
        }

        #region Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string), typeof(NavigationBar), null);
        #endregion


        #region Frame
        public Frame Frame
        {
            get { return (Frame)GetValue(FrameProperty); }
            set { SetValue(FrameProperty, value); }
        }

        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register("Frame",
            typeof(Frame), typeof(NavigationBar), new PropertyMetadata(null, (depobj, args) =>
            {
                ((NavigationBar)depobj)._updateBackButton();
            }));
        #endregion
    }
}
