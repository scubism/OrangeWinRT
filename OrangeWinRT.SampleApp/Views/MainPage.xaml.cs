using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace OrangeWinRT.SampleApp.Views
{

    public sealed partial class MainPage : OrangeWinRT.SampleApp.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void ScrollPaginatorButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(ScrollPaginatorExamplePage));
        }

        private void BreadcrumbsButton_Click(object sender, RoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(BreadcrumbsExamplePage));
        }

        private void ImageButtonButton_Click(object sender, RoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(ImageButtonExamplePage));
        }
    }
}
