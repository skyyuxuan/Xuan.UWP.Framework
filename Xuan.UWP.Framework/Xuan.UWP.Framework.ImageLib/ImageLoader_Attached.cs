using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Xuan.UWP.Framework.ImageLib
{
    public class ImageLoaderDependency : DependencyObject
    {
        private static object _lockObj = new object();
        public static string GetSource(UIElement d)
        {
            return (string)d.GetValue(SourceProperty);
        }

        public static void SetSource(UIElement d, string value)
        {
            d.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(object), typeof(ImageLoaderDependency), new PropertyMetadata(null, OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Image;
            SetSourceInternal(control, e.NewValue);
        }

        private static async void SetSourceInternal(Image image, object source)
        {
            image.Source = null;

            if (source == null)
            {
                return;
            }

            var imageSource = source as ImageSource;
            if (imageSource != null)
            {
                image.Source = imageSource;
                return;
            }

            Uri _uri = source as Uri;
            string _url = string.Empty;
            if (_uri == null)
            {
                _url = source as string ?? source.ToString();
            }
            else
            {
                _url = _uri.ToString();
            }

            try
            {
                using (var stream = await ImageLoader.GetInstance.GetImageStreamAsync(_url, null, new System.Threading.CancellationToken()))
                {
                    if (stream != null && stream.Size > 0)
                    {
                        lock (_lockObj)
                        {
                            var bit = new BitmapImage();
                            bit.SetSource(stream);
                            image.Source = bit;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("error");
                    }
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
