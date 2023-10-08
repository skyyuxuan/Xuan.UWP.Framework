using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Media3D;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Xuan.UWP.Framework.Controls
{
    [TemplateVisualState(Name = FLIPPEDY, GroupName = TRANSITIONSTATES)]
    [TemplateVisualState(Name = FLIPPEDYBACK, GroupName = TRANSITIONSTATES)]
    [TemplateVisualState(Name = FLIPPEDX, GroupName = TRANSITIONSTATES)]
    [TemplateVisualState(Name = FLIPPEDXBACK, GroupName = TRANSITIONSTATES)]
    [TemplateVisualState(Name = NORMAL, GroupName = TRANSITIONSTATES)]
    public sealed class Tile3D : Control
    {
        private const string TRANSITIONSTATES = "TransitionStates";
        private const string FLIPPEDY = "FlippedY";
        private const string FLIPPEDYBACK = "FlippedYBack";
        private const string FLIPPEDX = "FlippedX";
        private const string FLIPPEDXBACK = "FlippedXBack";
        private const string NORMAL = "Normal";

        private CompositeTransform3D RootTransform;
        private CompositeTransform3D ContentTransform;
        private CompositeTransform3D BackTransform;

        internal int _stallingCounter;


        private Grid _rootGrid;
        internal Grid RootGrid
        {
            get { return _rootGrid; }
            set
            {
                if (_rootGrid != null)
                {
                    _rootGrid.SizeChanged -= OnSizeChanged;
                }

                _rootGrid = value;

                if (_rootGrid != null)
                {
                    _rootGrid.SizeChanged += OnSizeChanged;
                }
            }
        }

        public Tile3D()
        {
            this.DefaultStyleKey = typeof(Tile3D);

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded; ;
        }


        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = e.NewSize;
            UpdateForSizeChanged(newSize.Width, newSize.Height);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Tile3DService.FinalizeReference(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Tile3DService.InitializeReference(this);
        }

        public DataTemplate BackContentTemplate
        {
            get { return (DataTemplate)GetValue(BackContentTemplateProperty); }
            set { SetValue(BackContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty BackContentTemplateProperty =
          DependencyProperty.Register(nameof(BackContentTemplate), typeof(DataTemplate), typeof(Tile3D), new PropertyMetadata(null));


        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateProperty =
          DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(Tile3D), new PropertyMetadata(null));

        public bool IsFrozen
        {
            get { return (bool)GetValue(IsFrozenProperty); }
            set { SetValue(IsFrozenProperty, value); }
        }
        public static readonly DependencyProperty IsFrozenProperty =
                 DependencyProperty.Register(nameof(IsFrozen), typeof(bool), typeof(Tile3D), new PropertyMetadata(false, new PropertyChangedCallback(OnIsFrozenChanged)));

        private static void OnIsFrozenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Tile3D tile = (Tile3D)obj;

            if ((bool)e.NewValue)
            {
                Tile3DService.FreezeTile3D(tile);
            }
            else
            {
                Tile3DService.UnfreezeTile3D(tile);
            }
        }


        internal Tile3DStates State
        {
            get { return (Tile3DStates)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private static readonly DependencyProperty StateProperty =
                DependencyProperty.Register(nameof(State), typeof(Tile3DStates), typeof(Tile3D), new PropertyMetadata(Tile3DStates.FlippedXBack, OnImageStateChanged));

        private static void OnImageStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((Tile3D)obj).UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            switch (State)
            {
                case Tile3DStates.FlippedY:
                case Tile3DStates.FlippedYBack:
                    BackTransform.RotationY = 270;
                    BackTransform.RotationX = 0;
                    break;

                case Tile3DStates.FlippedX:
                case Tile3DStates.FlippedXBack:
                    BackTransform.RotationX = 270;
                    BackTransform.RotationY = 0;
                    break;
            }
            VisualStateManager.GoToState(this, State.ToString(), true);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RootGrid = (Grid)GetTemplateChild("LayoutRoot");
            RootTransform = (CompositeTransform3D)GetTemplateChild("RootTransform");

            ContentTransform = (CompositeTransform3D)GetTemplateChild("ContentTransform");
            BackTransform = (CompositeTransform3D)GetTemplateChild("BackTransform");


        }

        private void UpdateForSizeChanged(double newWidth, double newHeight)
        {
            var centerX = newWidth / 2.0;
            var centerY = newHeight / 2.0;
            RootTransform.CenterX = centerX;
            RootTransform.CenterY = centerY;
            BackTransform.CenterX = centerX;
            BackTransform.CenterY = centerY;
            var centerZ = -newWidth / 2.0;
            RootTransform.CenterZ = centerZ;
            BackTransform.CenterZ = centerZ;
        }
    }
}
