using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;


namespace Xuan.UWP.Framework.Controls {
    [TemplatePart(Name = LAYOUTROOT, Type = typeof(Grid))]
    [TemplatePart(Name = MAINPAHTMENU, Type = typeof(PathMenuItem))]

    public sealed class PathMenu : ItemsControl {
        private const string LAYOUTROOT = "LayoutRoot";
        private const string MAINPAHTMENU = "MainPathMenu";

        private Grid _layout;
        private PathMenuItem _mainPathMenu;
        private Storyboard _openStoryboard;
        private Storyboard _hideStoryboard;

        private bool _isAnimating;
        private int _itemCount = -1;
        private float _duration = 0.5f;

        public PathMenu() {
            this.DefaultStyleKey = typeof(PathMenu);
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is PathMenuItem;
        }

        protected override DependencyObject GetContainerForItemOverride() {
            return new PathMenuItem();
        }

        protected override Size MeasureOverride(Size availableSize) {
            return base.MeasureOverride(availableSize);
        }
        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled) {
                this.LostFocus += OnLostFocus;
                if (_layout == null) {
                    _layout = this.GetTemplateChild(LAYOUTROOT) as Grid;
                }
                if (_mainPathMenu == null) {
                    _mainPathMenu = this.GetTemplateChild(MAINPAHTMENU) as PathMenuItem;
                }
                if (_mainPathMenu != null) {
                    _mainPathMenu.RenderTransform = new CompositeTransform();
                    _mainPathMenu.RenderTransformOrigin = new Point(0.5, 0.5);
                    _mainPathMenu.Click -= OnPathMenuClick;
                    _mainPathMenu.Click += OnPathMenuClick;
                }
                if (_layout != null) {
                    foreach (var item in Items) {
                        var menuItem = item as PathMenuItem;
                        if (menuItem != null) {
                            menuItem.RenderTransform = new CompositeTransform();
                            menuItem.Opacity = 0;
                            _layout.Children.Insert(0, menuItem);
                            menuItem.Click -= OnPathMenuClick;
                            menuItem.Click += OnPathMenuClick;
                        }
                    }
                    InitLayout();
                }
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e) {
            _isAnimating = false;
            if (IsOpen) {
                IsOpen = false;
            }
        }

        private void OnPathMenuClick(object sender, RoutedEventArgs e) {
            if (_isAnimating)
                return;
            IsOpen = !IsOpen;
        }

        //public Geometry Data
        //{
        //    get { return (Geometry)GetValue(DataProperty); }
        //    set { SetValue(DataProperty, value); }
        //}

        public int OrignAngle {
            get { return (int)GetValue(OrignAngleProperty); }
            set { SetValue(OrignAngleProperty, value); }
        }

        public PathMenuType MenuMode {
            get { return (PathMenuType)GetValue(MenuModeProperty); }
            set { SetValue(MenuModeProperty, value); }
        }

        public double Radius {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public bool IsOpen {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        //public static readonly DependencyProperty DataProperty =
        //       DependencyProperty.Register("Data", typeof(Geometry), typeof(PathMenu), new PropertyMetadata(null));


        public static readonly DependencyProperty MenuModeProperty =
                    DependencyProperty.Register("MenuMode", typeof(PathMenuType), typeof(PathMenu), new PropertyMetadata(PathMenuType.Right, (d, e) => ((PathMenu)d).OnMenuModePropertyChanged(d, e)));

        private void OnMenuModePropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != e.OldValue) {
                InitLayout();
            }
        }

        public static readonly DependencyProperty OrignAngleProperty =
        DependencyProperty.Register("OrignAngle", typeof(int), typeof(PathMenu), new PropertyMetadata(90, (d, e) => ((PathMenu)d).OnOrignAnglePropertyChanged(d, e)));

        private void OnOrignAnglePropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != e.OldValue) {
                InitLayout();
            }
        }
        public static readonly DependencyProperty RadiusProperty =
              DependencyProperty.Register("Radius", typeof(double), typeof(PathMenu), new PropertyMetadata(150d, (d, e) => ((PathMenu)d).OnRadiusPropertyChanged(d, e)));
        private void OnRadiusPropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != e.OldValue) {
                InitLayout();
            }
        }

        public static readonly DependencyProperty IsOpenProperty =
              DependencyProperty.Register("IsOpen", typeof(bool), typeof(PathMenu), new PropertyMetadata(false, (d, e) => ((PathMenu)d).OnIsOpenPropertyChanged(d, e)));
        private void OnIsOpenPropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != e.OldValue) {
                DoAnimation();
            }
        }
        private void InitLayout() {
            if (_layout == null)
                return;
            _itemCount = _layout.Children.Count();
            int dx = 1;
            int dy = 1;
            bool isTwoDirections = true;
            switch (MenuMode) {
                case PathMenuType.UpAndRight:
                    dx = 1;
                    dy = -1;
                    break;
                case PathMenuType.UpAndLeft:
                    dx = -1;
                    dy = -1;
                    break;
                case PathMenuType.DownAndRight:
                    dy = 1;
                    dx = 1;
                    break;
                case PathMenuType.DownAndLeft:
                    dy = 1;
                    dx = -1;
                    break;
                case PathMenuType.Up:
                    isTwoDirections = false;
                    dx = 0;
                    dy = -1;
                    break;
                case PathMenuType.Down:
                    isTwoDirections = false;
                    dx = 0;
                    dy = 1;
                    break;
                case PathMenuType.Left:
                    isTwoDirections = false;
                    dx = -1;
                    dy = 0;
                    break;
                case PathMenuType.Right:
                    isTwoDirections = false;
                    dx = 1; dy = 0;
                    break;
                default:
                    break;
            }
            double menuItemSpacing = 0;
            if (_itemCount - 1 > 0) {
                menuItemSpacing = (double)OrignAngle / (_itemCount - 1);
            }
            for (int i = 0; i < _itemCount; i++) {
                PathMenuItem pathMenuItem = _layout.Children[i] as PathMenuItem;
                if (pathMenuItem != null) {
                    if (isTwoDirections) {
                        pathMenuItem.X = dx * Radius * Math.Sin(i * Math.PI * menuItemSpacing / 180);
                        pathMenuItem.Y = dy * Radius * Math.Cos(i * Math.PI * menuItemSpacing / 180);
                    }
                    else {
                        var j = i + 1;
                        pathMenuItem.X = dx * j * Radius;
                        pathMenuItem.Y = dy * j * Radius;
                    }
                }
#if DEBUG
                System.Diagnostics.Debug.WriteLine(string.Format("i:{1},x:{0}", pathMenuItem.X.ToString(), i.ToString()));
                System.Diagnostics.Debug.WriteLine(string.Format("i:{1},y:{0}", pathMenuItem.Y.ToString(), i.ToString()));
#endif
            }

        }
        public void DoAnimation() {
            if (_isAnimating)
                return;
            _isAnimating = true;
            if (IsOpen) {
                if (_openStoryboard == null) {
                    _openStoryboard = new Storyboard();
                    for (int i = 0; i < _itemCount; i++) {
                        PathMenuItem pathMenuItem = Items[i] as PathMenuItem;
                        if (pathMenuItem != null) {
                            double num = 50.0 / (double)_itemCount * (double)i / 100.0;
                            double num2 = 40.0 / (double)_itemCount * (double)i / 100.0;
                            pathMenuItem.RenderTransformOrigin = new Point(0.5, 0.5);

                            _openStoryboard.Children.Add(GetOpenTimeline(pathMenuItem, pathMenuItem.X, num, num2, _duration, true));
                            _openStoryboard.Children.Add(GetOpenTimeline(pathMenuItem, pathMenuItem.Y, num, num2, _duration, false));
                            _openStoryboard.Children.Add(GetOpacityAnimation(pathMenuItem, _duration, 0, 1));
                            _openStoryboard.Children.Add(GetRotateTimeline(pathMenuItem, num, num2, _duration));
                        }
                    }
                    if (_mainPathMenu != null)
                        _openStoryboard.Children.Add(GetRotateAnimation(_mainPathMenu, 0, 45));
                    _openStoryboard.Completed += OnStoryboardCompleted;
                }
                _openStoryboard.Begin();
            }
            else {
                if (_hideStoryboard == null) {
                    _hideStoryboard = new Storyboard();
                    for (int i = 0; i < _itemCount; i++) {
                        PathMenuItem pathMenuItem = Items[i] as PathMenuItem;
                        if (pathMenuItem != null) {
                            double num = 50.0 / (double)_itemCount * (double)i / 100.0;
                            double num2 = 40.0 / (double)_itemCount * (double)i / 100.0;
                            pathMenuItem.RenderTransformOrigin = new Point(0.5, 0.5);

                            _hideStoryboard.Children.Add(GetHideTimeline(pathMenuItem, pathMenuItem.X, num, num2, _duration, true));
                            _hideStoryboard.Children.Add(GetHideTimeline(pathMenuItem, pathMenuItem.Y, num, num2, _duration, false));
                            _hideStoryboard.Children.Add(GetOpacityAnimation(pathMenuItem, _duration, 1, 0));
                            _hideStoryboard.Children.Add(GetRotateTimeline(pathMenuItem, num, num2, _duration));

                        }
                    }
                    if (_mainPathMenu != null)
                        _hideStoryboard.Children.Add(GetRotateAnimation(_mainPathMenu, 45, 0));
                    _hideStoryboard.Completed += OnStoryboardCompleted;
                }
                _hideStoryboard.Begin();
            }
        }

        private void OnStoryboardCompleted(object sender, object e) {
            _isAnimating = false;
        }

        private DoubleAnimationUsingKeyFrames GetOpenTimeline(UIElement element, double position, double delay, double delay2, float duration, bool isX = false) {
            DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame easingDoubleKeyFrame = new EasingDoubleKeyFrame();
            easingDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0));
            easingDoubleKeyFrame.Value = 0.0;
            EasingDoubleKeyFrame easingDoubleKeyFrame2 = easingDoubleKeyFrame;
            EasingDoubleKeyFrame arg2 = easingDoubleKeyFrame2;

            ExponentialEase exponentialEase = new ExponentialEase();
            exponentialEase.EasingMode = EasingMode.EaseOut;
            arg2.EasingFunction = exponentialEase;

            DiscreteDoubleKeyFrame discreteDoubleKeyFrame = new DiscreteDoubleKeyFrame();
            discreteDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration * (0.15 + delay)));
            discreteDoubleKeyFrame.Value = 0.0;
            DiscreteDoubleKeyFrame discreteDoubleKeyFrame2 = discreteDoubleKeyFrame;
            EasingDoubleKeyFrame easingDoubleKeyFrame3 = new EasingDoubleKeyFrame();
            easingDoubleKeyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration * (0.45 + delay2)));
            easingDoubleKeyFrame3.Value = position * 1.2;
            EasingDoubleKeyFrame easingDoubleKeyFrame4 = easingDoubleKeyFrame3;
            EasingDoubleKeyFrame arg4 = easingDoubleKeyFrame4;

            ExponentialEase exponentialEase2 = new ExponentialEase();
            exponentialEase2.EasingMode = EasingMode.EaseInOut;
            arg4.EasingFunction = exponentialEase2;
            EasingDoubleKeyFrame easingDoubleKeyFrame5 = new EasingDoubleKeyFrame();

            easingDoubleKeyFrame5.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration * 0.95));
            easingDoubleKeyFrame5.Value = position * 0.95;
            EasingDoubleKeyFrame easingDoubleKeyFrame6 = easingDoubleKeyFrame5;
            EasingDoubleKeyFrame arg6 = easingDoubleKeyFrame6;
            ExponentialEase exponentialEase3 = new ExponentialEase();
            exponentialEase3.EasingMode = EasingMode.EaseInOut;
            arg6.EasingFunction = exponentialEase3;

            DiscreteDoubleKeyFrame discreteDoubleKeyFrame3 = new DiscreteDoubleKeyFrame();
            discreteDoubleKeyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration));
            discreteDoubleKeyFrame3.Value = position;
            DiscreteDoubleKeyFrame discreteDoubleKeyFrame4 = discreteDoubleKeyFrame3;
            doubleAnimationUsingKeyFrames.KeyFrames.Add(easingDoubleKeyFrame2);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(discreteDoubleKeyFrame2);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(easingDoubleKeyFrame4);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(easingDoubleKeyFrame6);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(discreteDoubleKeyFrame4);
            string text = isX ? "(UIElement.RenderTransform).(CompositeTransform.TranslateX)" : "(UIElement.RenderTransform).(CompositeTransform.TranslateY)";
            Storyboard.SetTarget(doubleAnimationUsingKeyFrames, element);
            Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, text);
            return doubleAnimationUsingKeyFrames;
        }

        private DoubleAnimationUsingKeyFrames GetHideTimeline(UIElement element, double position, double delay, double delay2, float duration, bool isX = false) {
            DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            DiscreteDoubleKeyFrame discreteDoubleKeyFrame = new DiscreteDoubleKeyFrame();
            discreteDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((double)duration * (0.15 + delay)));
            discreteDoubleKeyFrame.Value = position;
            DiscreteDoubleKeyFrame discreteDoubleKeyFrame2 = discreteDoubleKeyFrame;
            EasingDoubleKeyFrame easingDoubleKeyFrame = new EasingDoubleKeyFrame();
            easingDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((double)duration * (0.45 + delay2)));
            easingDoubleKeyFrame.Value = position * 0.3;
            EasingDoubleKeyFrame easingDoubleKeyFrame2 = easingDoubleKeyFrame;
            EasingDoubleKeyFrame arg = easingDoubleKeyFrame2;
            ExponentialEase exponentialEase = new ExponentialEase();
            exponentialEase.EasingMode = EasingMode.EaseIn;
            arg.EasingFunction = exponentialEase;
            EasingDoubleKeyFrame easingDoubleKeyFrame3 = new EasingDoubleKeyFrame();
            easingDoubleKeyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((double)duration * 0.95));
            easingDoubleKeyFrame3.Value = 0.0;
            EasingDoubleKeyFrame easingDoubleKeyFrame4 = easingDoubleKeyFrame3;
            EasingDoubleKeyFrame arg2 = easingDoubleKeyFrame4;
            ExponentialEase exponentialEase2 = new ExponentialEase();
            exponentialEase2.EasingMode = EasingMode.EaseInOut;
            arg2.EasingFunction = exponentialEase2;
            DiscreteDoubleKeyFrame discreteDoubleKeyFrame3 = new DiscreteDoubleKeyFrame();
            discreteDoubleKeyFrame3.KeyTime = (KeyTime.FromTimeSpan(TimeSpan.FromSeconds((double)duration)));
            discreteDoubleKeyFrame3.Value = (0.0);
            DiscreteDoubleKeyFrame discreteDoubleKeyFrame4 = discreteDoubleKeyFrame3;
            doubleAnimationUsingKeyFrames.KeyFrames.Add(discreteDoubleKeyFrame2);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(easingDoubleKeyFrame2);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(easingDoubleKeyFrame4);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(discreteDoubleKeyFrame4);
            string text = isX ? "(UIElement.RenderTransform).(CompositeTransform.TranslateX)" : "(UIElement.RenderTransform).(CompositeTransform.TranslateY)";
            Storyboard.SetTarget(doubleAnimationUsingKeyFrames, element);
            Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, text);
            return doubleAnimationUsingKeyFrames;
        }

        private DoubleAnimation GetRotateTimeline(UIElement element, double time1, double time2, float duration) {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.BeginTime = TimeSpan.FromSeconds(duration * (0.15 + time1));
            doubleAnimation.From = 0.0;
            doubleAnimation.To = 720.0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(duration * (0.45 + time2));
            Storyboard.SetTarget(doubleAnimation, element);
            Storyboard.SetTargetProperty(doubleAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            return doubleAnimation;
        }

        private DoubleAnimation GetRotateAnimation(UIElement element, double from, double to) {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = from;
            doubleAnimation.To = to;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            Storyboard.SetTarget(doubleAnimation, element);
            Storyboard.SetTargetProperty(doubleAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            return doubleAnimation;
        }

        private DoubleAnimation GetOpacityAnimation(UIElement element, double duration, double from, double to) {
            var da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(duration);
            da.From = from;
            da.To = to;
            da.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(da, element);
            Storyboard.SetTargetProperty(da, "UIElement.Opacity");
            return da;
        }

    }

    public enum PathMenuType {
        UpAndRight = 0,
        UpAndLeft = 1,
        DownAndRight = 2,
        DownAndLeft = 3,
        Up = 4,
        Down = 5,
        Left = 6,
        Right = 7
    }
}
