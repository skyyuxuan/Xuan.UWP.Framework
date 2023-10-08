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
    public class RatingItem : ContentControl
    {
        public RatingItem()
        {
            DefaultStyleKey = typeof(RatingItem);
        }
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty StrokeThicknessProperty =
          DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RatingItem), new PropertyMetadata(0d));

        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }
        public static readonly DependencyProperty PathDataProperty =
          DependencyProperty.Register("PathData", typeof(Geometry), typeof(RatingItem), new PropertyMetadata(null));
    }
}
