using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Xuan.UWP.Framework.Controls
{
    [TemplatePart(Name = Rating.FILLEDCLIPELEMENT, Type = typeof(Border))]
    [TemplatePart(Name = Rating.FILLEDGRIDELEMENT, Type = typeof(Grid))]
    [TemplatePart(Name = Rating.UNFILLEDGRIDELEMENT, Type = typeof(Grid))]
    [TemplatePart(Name = Rating.DRAGBORDERELEMENT, Type = typeof(Border))]
    [TemplatePart(Name = Rating.DRAGTEXTBLOCKELEMENT, Type = typeof(TextBlock))]
    [TemplateVisualState(Name = Rating.DRAGHELPERCOLLAPSED, GroupName = Rating.DRAGHELPERSTATES)]
    [TemplateVisualState(Name = Rating.DRAGHELPERVISIBLE, GroupName = Rating.DRAGHELPERSTATES)]
    public class Rating : Control
    {
        private const string FILLEDCLIPELEMENT = "FilledClipElement";
        private const string FILLEDGRIDELEMENT = "FilledGridElement";
        private const string UNFILLEDGRIDELEMENT = "UnfilledGridElement";
        private const string DRAGBORDERELEMENT = "DragBorderElement";
        private const string DRAGTEXTBLOCKELEMENT = "DragTextBlockElement";
        private const string DRAGHELPERSTATES = "DragHelperStates";
        private const string DRAGHELPERCOLLAPSED = "Collapsed";
        private const string DRAGHELPERVISIBLE = "Visible";

        private Border _filledClipElement;
        private Grid _filledGridElement;
        private Grid _unfilledGridElement;
        private Border _dragBorderElement;
        private TextBlock _dragTextBlockElement;
        private RectangleGeometry _clippingMask;

        private List<RatingItem> _filledItemCollection = new List<RatingItem>();
        private List<RatingItem> _unfilledItemCollection = new List<RatingItem>();
        public event EventHandler ValueChanged;

        public Rating()
        {
            DefaultStyleKey = typeof(Rating);
            SizeChanged += OnSizeChanged;
            ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX;
            ManipulationStarted += OnManipulationStarted;
            ManipulationDelta += OnManipulationDelta;
            ManipulationCompleted += OnManipulationCompleted;
            AdjustNumberOfRatingItems();
            SynchronizeGrids();
            UpdateClippingMask();
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            if (!ReadOnly)
            {
                PerformValueCalculation(e.GetPosition(this), this);
                UpdateDragHelper();
            }
        }



        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateClippingMask();
        }



        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!ReadOnly)
            {
                PerformValueCalculation(e.Position, e.Container);
                UpdateDragHelper();
                if (ShowSelectionHelper)
                {
                    ChangeDragHelperVisibility(true);
                }
            }
        }

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (!ReadOnly)
            {
                PerformValueCalculation(e.Position, e.Container);
                UpdateDragHelper();
            }
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!ReadOnly)
            {
                PerformValueCalculation(e.Position, e.Container);
            }
            ChangeDragHelperVisibility(false);
        }




        protected override void OnApplyTemplate()
        {
            _filledClipElement = GetTemplateChild(FILLEDCLIPELEMENT) as Border;
            _filledGridElement = GetTemplateChild(FILLEDGRIDELEMENT) as Grid;
            _unfilledGridElement = GetTemplateChild(UNFILLEDGRIDELEMENT) as Grid;
            _dragBorderElement = GetTemplateChild(DRAGBORDERELEMENT) as Border;
            _dragTextBlockElement = GetTemplateChild(DRAGTEXTBLOCKELEMENT) as TextBlock;

            if (_filledClipElement != null)
            {

                _filledClipElement.Clip = _clippingMask;
            }

            if (_dragBorderElement != null)
            {
                _dragBorderElement.RenderTransform = new TranslateTransform();
            }
            VisualStateManager.GoToState(this, "Collapsed", false);
            SynchronizeGrids();
        }

        #region Grid Modifiers and Helpers


        private void ChangeDragHelperVisibility(bool isVisible)
        {
            if (_dragBorderElement == null)
            {
                return;
            }

            if (isVisible)
            {
                VisualStateManager.GoToState(this, DRAGHELPERVISIBLE, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DRAGHELPERCOLLAPSED, true);
            }
        }


        private void UpdateDragHelper()
        {

            if (RatingItemCount == 0)
            {
                return;
            }

            string textBlockFormatString;
            if (AllowHalfItemIncrement)
            {
                textBlockFormatString = "F1";
            }
            else
            {
                textBlockFormatString = "F0";
            }

            if (_dragTextBlockElement != null)
            {
                _dragTextBlockElement.Text = Value.ToString(textBlockFormatString, CultureInfo.CurrentCulture);
            }

            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                if (_dragBorderElement != null)
                {
                    double distanceToCenterOfDragBorder = (_dragBorderElement.ActualWidth) / 2;
                    double distanceToCenterOfRatingItem = (_filledItemCollection[0].ActualWidth) / 2;

                    TranslateTransform t = (TranslateTransform)_dragBorderElement.RenderTransform;

                    if (!AllowHalfItemIncrement && !AllowSelectingZero)
                    {
                        t.X = (Value / RatingItemCount) * ActualWidth - distanceToCenterOfDragBorder - distanceToCenterOfRatingItem;
                    }
                    else
                    {
                        t.X = (Value / RatingItemCount) * ActualWidth - distanceToCenterOfDragBorder;
                    }

                    t.Y = -(ActualHeight / 2 + 15);
                }
            }
            else
            {
                if (_dragBorderElement != null)
                {
                    double distanceToCenterOfDragBorder = (_dragBorderElement.ActualHeight) / 2;
                    double distanceToCenterOfRatingItem = (_filledItemCollection[0].ActualHeight) / 2;


                    TranslateTransform t = (TranslateTransform)_dragBorderElement.RenderTransform;

                    if (!AllowHalfItemIncrement && !AllowSelectingZero)
                    {
                        t.Y = (Value / RatingItemCount) * ActualHeight - distanceToCenterOfDragBorder - distanceToCenterOfRatingItem;
                    }
                    else
                    {
                        t.Y = (Value / RatingItemCount) * ActualHeight - distanceToCenterOfDragBorder;
                    }

                    t.X = -(ActualWidth / 2 + 15);
                }
            }
        }


        private void PerformValueCalculation(Point location, UIElement locationRelativeSource)
        {
            GeneralTransform gt = locationRelativeSource.TransformToVisual(this);
            location = gt.TransformPoint(location);

            int numberOfPositions = _filledItemCollection.Count;

            if (AllowHalfItemIncrement)
            {
                numberOfPositions *= 2;
            }

            double newValue;
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                newValue = Math.Ceiling(location.X / ActualWidth * numberOfPositions);
            }
            else
            {
                newValue = Math.Ceiling(location.Y / ActualHeight * numberOfPositions);
            }

            if (!AllowSelectingZero && newValue <= 0)
            {
                newValue = 1;
            }

            Value = newValue;
        }


        private void UpdateClippingMask()
        {
            Rect newRect;

            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                double widthMinusBorder = ActualWidth - BorderThickness.Right - BorderThickness.Left;
                newRect = new Rect(0, 0, widthMinusBorder * (Value / RatingItemCount), ActualHeight);
            }
            else
            {
                double heightMinusBorder = ActualHeight - BorderThickness.Top - BorderThickness.Bottom;
                newRect = new Rect(0, 0, ActualWidth, heightMinusBorder * (Value / RatingItemCount));
            }

            RectangleGeometry rGeo = _clippingMask;


            if (rGeo != null)
            {
                rGeo.Rect = newRect;
            }
            else
            {
                rGeo = new RectangleGeometry();
                rGeo.Rect = newRect;
                _clippingMask = rGeo;
            }
        }


        private static RatingItem BuildNewRatingItem(Style s)
        {
            RatingItem ri = new RatingItem();
            if (s != null)
            {
                ri.Style = s;
            }
            return ri;
        }


        private void AdjustNumberOfRatingItems()
        {
            while (_filledItemCollection.Count > RatingItemCount)
            {
                _filledItemCollection.RemoveAt(0);
            }

            while (_unfilledItemCollection.Count > RatingItemCount)
            {
                _unfilledItemCollection.RemoveAt(0);
            }

            while (_filledItemCollection.Count < RatingItemCount)
            {
                _filledItemCollection.Add(BuildNewRatingItem(FilledItemStyle));
            }

            while (_unfilledItemCollection.Count < RatingItemCount)
            {
                _unfilledItemCollection.Add(BuildNewRatingItem(UnfilledItemStyle));
            }
        }


        private void SynchronizeGrid(Grid grid, IList<RatingItem> ratingItemList)
        {
            if (grid == null)
            {
                return;
            }

            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {

                while (grid.ColumnDefinitions.Count < ratingItemList.Count)
                {
                    ColumnDefinition cD = new ColumnDefinition();
                    cD.Width = new GridLength(1, GridUnitType.Star);
                    grid.ColumnDefinitions.Add(cD);
                }

                grid.Children.Clear();
                for (int i = 0; i < ratingItemList.Count; i++)
                {
                    grid.Children.Add(ratingItemList[i]);
                    Grid.SetColumn(ratingItemList[i], i);
                    Grid.SetRow(ratingItemList[i], 0);
                }
            }
            else
            {
                while (grid.RowDefinitions.Count < ratingItemList.Count)
                {
                    RowDefinition rD = new RowDefinition();
                    rD.Height = new GridLength(1, GridUnitType.Star);
                    grid.RowDefinitions.Add(rD);
                }

                grid.Children.Clear();
                for (int i = 0; i < ratingItemList.Count; i++)
                {
                    grid.Children.Add(ratingItemList[i]);
                    Grid.SetRow(ratingItemList[i], i);
                    Grid.SetColumn(ratingItemList[i], 0);
                }

            }
        }


        private void SynchronizeGrids()
        {
            SynchronizeGrid(_unfilledGridElement, _unfilledItemCollection);
            SynchronizeGrid(_filledGridElement, _filledItemCollection);
        }

        #endregion

        #region public Style FilledItemStyle

        public Style FilledItemStyle
        {
            get { return (Style)GetValue(FilledItemStyleProperty); }
            set { SetValue(FilledItemStyleProperty, value); }
        }


        public static readonly DependencyProperty FilledItemStyleProperty =
            DependencyProperty.Register("FilledItemStyle",
            typeof(Style), typeof(Rating),
            new PropertyMetadata(null, OnFilledItemStyleChanged));

        private static void OnFilledItemStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnFilledItemStyleChanged();
        }

        private void OnFilledItemStyleChanged()
        {
            foreach (RatingItem ri in _filledItemCollection)
            {
                ri.Style = FilledItemStyle;
            }
        }
        #endregion

        #region public style UnfilledItemStyle

        public Style UnfilledItemStyle
        {
            get { return (Style)GetValue(UnfilledItemStyleProperty); }
            set { SetValue(UnfilledItemStyleProperty, value); }
        }


        public static readonly DependencyProperty UnfilledItemStyleProperty =
            DependencyProperty.Register("UnfilledItemStyle",
            typeof(Style), typeof(Rating),
            new PropertyMetadata(null, OnUnfilledItemStyleChanged));
        private static void OnUnfilledItemStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnUnfilledItemStyleChanged();
        }

        private void OnUnfilledItemStyleChanged()
        {
            foreach (RatingItem ri in _unfilledItemCollection)
            {

                ri.Style = UnfilledItemStyle;
            }
        }
        #endregion

        #region public int RatingItemCount
        public int RatingItemCount
        {
            get { return (int)GetValue(RatingItemCountProperty); }
            set { SetValue(RatingItemCountProperty, value); }
        }

        /// <summary>
        /// Identifies the RatingItemCount dependency property.
        /// </summary>
        public static readonly DependencyProperty RatingItemCountProperty =
            DependencyProperty.Register("RatingItemCount",
                typeof(int), typeof(Rating),
                new PropertyMetadata(5, OnRatingItemCountChanged));

        private static void OnRatingItemCountChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnRatingItemCountChanged();
        }

        private void OnRatingItemCountChanged()
        {
            if (RatingItemCount <= 0)
            {
                RatingItemCount = 0;
            }

            AdjustNumberOfRatingItems();
            SynchronizeGrids();
        }
        #endregion

        #region public double Value

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Property traditionally named value")]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                if (ValueChanged != null)
                {
                    ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(double), typeof(Rating),
                new PropertyMetadata(0.0, OnValueChanged));

        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (Value > RatingItemCount || Value < 0)
            {
                Value = Math.Max(0, Math.Min(RatingItemCount, Value));
            }
            UpdateClippingMask();
        }
        #endregion

        #region public boolean ReadOnly

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }


        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(Rating), new PropertyMetadata(false));
        #endregion

        #region public boolean AllowHalfItemIncrement

        public bool AllowHalfItemIncrement
        {
            get { return (bool)GetValue(AllowHalfItemIncrementProperty); }
            set { SetValue(AllowHalfItemIncrementProperty, value); }
        }


        public static readonly DependencyProperty AllowHalfItemIncrementProperty =
            DependencyProperty.Register("AllowHalfItemIncrement", typeof(bool), typeof(Rating), new PropertyMetadata(false));
        #endregion

        #region public boolean AllowSelectingZero

        public bool AllowSelectingZero
        {
            get { return (bool)GetValue(AllowSelectingZeroProperty); }
            set { SetValue(AllowSelectingZeroProperty, value); }
        }


        public static readonly DependencyProperty AllowSelectingZeroProperty =
            DependencyProperty.Register("AllowSelectingZero", typeof(bool), typeof(Rating), new PropertyMetadata(false));
        #endregion

        #region public boolean ShowSelectionHelper

        public bool ShowSelectionHelper
        {
            get { return (bool)GetValue(ShowSelectionHelperProperty); }
            set { SetValue(ShowSelectionHelperProperty, value); }
        }


        public static readonly DependencyProperty ShowSelectionHelperProperty =
            DependencyProperty.Register("ShowSelectionHelperItems", typeof(bool), typeof(Rating), new PropertyMetadata(false));
        #endregion

        #region public Orientation Orientation

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }


        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Rating), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnOrientationChanged();
        }

        private void OnOrientationChanged()
        {
            SynchronizeGrids();
            UpdateClippingMask();
        }
        #endregion

    }
}
