using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Xuan.UWP.Framework.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Xuan.UWP.Framework.TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //using (var stream = await ImageLib.ImageLoader.GetInstance.GetImageStreamAsync(@"http://ecx.images-amazon.com/images/I/512Pd6birKL.jpg",
            //    null, new System.Threading.CancellationToken()))
            //{
            //    if (stream != null && stream.Size > 0)
            //    {
            //        var bit = new BitmapImage();
            //        await bit.SetSourceAsync(stream);
            //        img.Source = bit;
            //    }
            //}
            List<ImageItem> imageList = new List<ImageItem>();
            for (int i = 0; i < 100; i++)
            {
                imageList.Add(new ImageItem() { Url = @"http://ecx.images-amazon.com/images/I/512Pd6birKL.jpg" });
            }
            listView.ItemsSource = imageList;
        }

        public class ImageItem : DataModelBase
        {
            private string _url;
            public string Url
            {
                get { return _url; }
                set { SetProperty(ref _url, value); } 
            }
        }
    }
}
