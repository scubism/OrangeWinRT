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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace OrangeWinRT.SampleApp.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ImageButtonExamplePage : Page
    {
        public ImageButtonExamplePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、通常、ページを構成するために使用します。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Button1 clicked");
            await dialog.ShowAsync();
        }

        private async void ImageButton_Click_2(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Voice button clicked");
            await dialog.ShowAsync();
        }

        private async void ImageButton_Click_3(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Right arrow icon clicked");
            await dialog.ShowAsync();
        }
    }
}
