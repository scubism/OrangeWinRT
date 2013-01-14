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
using System.Collections.ObjectModel;

using OrangeWinRT.SampleApp.DataModel;


// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace OrangeWinRT.SampleApp.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ScrollIndicatorExamplePage : Page
    {
        public ScrollIndicatorExamplePage()
        {
            this.InitializeComponent();
        }

        public class Group : OrangeWinRT.SampleApp.Common.BindableBase
        {
            public ObservableCollection<object> Items;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var groups = SampleDataSource.GetGroups("AllGroups");
            var source = new CollectionViewSource();
            source.Source = groups;
            source.IsSourceGrouped = true;
            source.ItemsPath = new PropertyPath("TopItems");
            this.GrieView1.DataContext = source;

            this.ListView1.DataContext = groups.First().TopItems;
            this.ListView2.DataContext = groups.First().TopItems;
        }

        private void GrieView1_Loaded(object sender, RoutedEventArgs e)
        {
            // NOTE: cannot use VirtualizingStackPanel with pagination ..
            var elem = VisualTreeHelper.GetChild(sender as DependencyObject, 0);
            this.GridView1ScrollIndicator.ScrollViewer = VisualTreeHelper.GetChild(elem, 0) as ScrollViewer;
        }

        private void ListView1_Loaded(object sender, RoutedEventArgs e)
        {
            var elem = VisualTreeHelper.GetChild(sender as DependencyObject, 0);
            this.ListView1ScrollIndicator.ScrollViewer = VisualTreeHelper.GetChild(elem, 0) as ScrollViewer;
        }

        private void ListView2_Loaded(object sender, RoutedEventArgs e)
        {
            var elem = VisualTreeHelper.GetChild(sender as DependencyObject, 0);
            this.ListView2ScrollIndicator.ScrollViewer = VisualTreeHelper.GetChild(elem, 0) as ScrollViewer;
        }

    }
}
