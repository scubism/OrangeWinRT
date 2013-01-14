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

using Windows.UI.Popups;

namespace OrangeWinRT.SampleApp.Views
{
    public sealed partial class BreadcrumbsExamplePage : Page
    {
        public BreadcrumbsExamplePage()
        {
            this.InitializeComponent();
        }

        private List<object> _breadcrumbs;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._breadcrumbs = new List<object>();
            this._breadcrumbs.Add("home");
            this._breadcrumbs.Add("category1");
            this._breadcrumbs.Add("category1-1");
            this.Breadcrumbs1.Loaded += (_s, _e) =>
            {
                this.Breadcrumbs1.DataContext = this._breadcrumbs;
            };
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string breadcrumb = ((FrameworkElement)e.OriginalSource).DataContext as string;
            var dialog = new MessageDialog(breadcrumb);
            await dialog.ShowAsync();
        }
    }
}
