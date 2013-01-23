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

namespace OrangeWinRT.SampleApp.Views
{
    public sealed partial class ScrollPaginatorExamplePage : Page
    {
        public ScrollPaginatorExamplePage()
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
            this.GridView1.DataContext = source;

            this.ListView1.DataContext = groups.First().TopItems;
            this.ListView2.DataContext = groups.First().TopItems;
        }
    }
}
