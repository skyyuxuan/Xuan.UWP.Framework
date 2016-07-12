using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Xuan.UWP.Framework.Controls
{
    public class PathMenuItem : Button
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
                  DependencyProperty.Register("Data", typeof(Geometry), typeof(PathMenuItem), new PropertyMetadata(null));

        public static readonly DependencyProperty CornerRadiusProperty =
                 DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PathMenuItem), new PropertyMetadata(0));

        public PathMenuItem()
        {
            this.DefaultStyleKey = typeof(PathMenuItem);
        } 
      
    }
}
