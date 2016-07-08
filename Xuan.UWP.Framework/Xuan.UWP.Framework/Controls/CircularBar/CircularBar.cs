using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Xuan.UWP.Framework.Controls
{
    [TemplatePart(Name = CircularBar.PathRoot, Type = typeof(Path))]
    [TemplatePart(Name = CircularBar.PathBack, Type = typeof(Path))]
    public class CircularBar : ContentControl
    {
        private const string PathRoot = "TC_PathRoot";
        private const string PathBack = "TC_PathBack";

        private Path pathRoot;
        private Path pathBack;
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
        public static readonly DependencyProperty PercentageProperty =
           DependencyProperty.Register("Percentage", typeof(double), typeof(CircularBar), new PropertyMetadata(65d, OnPercentageChanged));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty StrokeThicknessProperty =
           DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularBar), new PropertyMetadata(5d));
        public Brush SegmentBrush
        {
            get { return (Brush)GetValue(SegmentBrushProperty); }
            set { SetValue(SegmentBrushProperty, value); }
        }
        public static readonly DependencyProperty SegmentBrushProperty =
          DependencyProperty.Register("SegmentBrush", typeof(Brush), typeof(CircularBar), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush BackSegmentBrush
        {
            get { return (Brush)GetValue(BackSegmentBrushProperty); }
            set { SetValue(BackSegmentBrushProperty, value); }
        }
        public static readonly DependencyProperty BackSegmentBrushProperty =
          DependencyProperty.Register("BackSegmentBrush", typeof(Brush), typeof(CircularBar), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public double BackOpacity
        {
            get { return (double)GetValue(BackOpacityProperty); }
            set { SetValue(BackOpacityProperty, value); }
        }
        public static readonly DependencyProperty BackOpacityProperty =
          DependencyProperty.Register("BackOpacity", typeof(double), typeof(CircularBar), new PropertyMetadata(0.5));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public static readonly DependencyProperty RadiusProperty =
           DependencyProperty.Register("Radius", typeof(double), typeof(CircularBar), new PropertyMetadata(25d, OnPropertyChanged));
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
                  DependencyProperty.Register("Angle", typeof(double), typeof(CircularBar), new PropertyMetadata(120d, OnPropertyChanged));

        private static void OnPercentageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var circle = (CircularBar)sender;
            circle.Angle = (circle.Percentage * 360) / 100;
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var circle = (CircularBar)sender;
            circle.RenderArc();
        }

        public void RenderArc()
        {
            if (pathRoot == null)
                pathRoot = this.GetTemplateChild(PathRoot) as Path;
            if (pathRoot == null)
                return;
            var startPoint = new Point(Radius, 0);
            var endPoint = ComputeCartesianCoordinate(Angle, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;
            pathRoot.Width = Radius * 2 + StrokeThickness;
            pathRoot.Height = Radius * 2 + StrokeThickness;
            pathRoot.Margin = new Thickness(StrokeThickness, StrokeThickness, 0, 0);

            var largeArc = Angle > 180.0;

            var outerArcSize = new Size(Radius, Radius);
            var pg = new PathGeometry();
            var pf = new PathFigure();
            var seg = new ArcSegment();
            seg.SweepDirection = SweepDirection.Clockwise;
            pf.StartPoint = startPoint;

            if (Math.Abs(startPoint.X - Math.Round(endPoint.X)) < .1 && Math.Abs(startPoint.Y - Math.Round(endPoint.Y)) < .1)
                endPoint.X -= 0.01;

            seg.Point = endPoint;
            seg.Size = outerArcSize;
            seg.IsLargeArc = largeArc;

            pf.Segments.Add(seg);
            pg.Figures.Add(pf);
            pathRoot.Data = pg;
            CalculatePathBackGeometry();//计算阴影部分
        }

        private void CalculatePathBackGeometry()
        {
            if (pathBack == null)
                pathBack = this.GetTemplateChild(PathBack) as Path;
            if (pathBack == null)
                return;
            var startPoint = new Point(Radius, 0);
            var endPoint = ComputeCartesianCoordinate(360, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;
            pathBack.Width = Radius * 2 + StrokeThickness;
            pathBack.Height = Radius * 2 + StrokeThickness;
            pathBack.Margin = new Thickness(StrokeThickness, StrokeThickness, 0, 0);
            var outerArcSize = new Size(Radius, Radius);
            var pg = new PathGeometry();
            var pf = new PathFigure();
            var seg = new ArcSegment();
            seg.SweepDirection = SweepDirection.Clockwise;
            pf.StartPoint = startPoint;

            if (Math.Abs(startPoint.X - Math.Round(endPoint.X)) < .1 && Math.Abs(startPoint.Y - Math.Round(endPoint.Y)) < .1)
                endPoint.X -= 0.01;

            seg.Point = endPoint;
            seg.Size = outerArcSize;
            seg.IsLargeArc = true;

            pf.Segments.Add(seg);
            pg.Figures.Add(pf);
            pathBack.Data = pg;
        }
        private Point ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            var angleRad = (Math.PI / 180.0) * (angle - 90);

            var x = radius * Math.Cos(angleRad);
            var y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (pathRoot == null)
            {
                pathRoot = this.GetTemplateChild(PathRoot) as Path;
            }
            if (pathBack == null)
            {
                OnPropertyChanged(this, null);
            }

        }
        public CircularBar()
        {
            this.DefaultStyleKey = typeof(CircularBar);
        }

    }
}
