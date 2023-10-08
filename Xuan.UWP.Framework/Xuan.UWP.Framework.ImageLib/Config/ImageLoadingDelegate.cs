using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Xuan.UWP.Framework.ImageLib.Config
{
    public delegate void ImageLoadingDelegate(ImageLoadingState state, string url, Image imageView, BitmapImage bitmap);
    public enum ImageLoadingState
    {
        Loading = 0,
        Completed = 1,
        Error = 2,
    }
}
