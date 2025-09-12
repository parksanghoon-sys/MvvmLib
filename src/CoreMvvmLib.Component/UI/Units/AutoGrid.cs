using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CoreMvvmLib.Component.UI.Units
{
    public class AutoGrid : Grid
    {
        public static readonly DependencyProperty ChildHorizontalAlignmentProperty = DependencyProperty.Register("ChildHorizontalAlignment", typeof(HorizontalAlignment?), typeof(AutoGrid), new FrameworkPropertyMetadata((HorizontalAlignment?)null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnChildHorizontalAlignmentChanged));

        public static readonly DependencyProperty ChildMarginProperty = DependencyProperty.Register("ChildMargin", typeof(Thickness?), typeof(AutoGrid), new FrameworkPropertyMetadata((Thickness?)null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnChildMarginChanged));

        public static readonly DependencyProperty ChildVerticalAlignmentProperty = DependencyProperty.Register("ChildVerticalAlignment", typeof(VerticalAlignment?), typeof(AutoGrid), new FrameworkPropertyMetadata((VerticalAlignment?)null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnChildVerticalAlignmentChanged));

        public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.RegisterAttached("ColumnCount", typeof(int), typeof(AutoGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure, ColumnCountChanged));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns", typeof(string), typeof(AutoGrid), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure, ColumnsChanged));

        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.RegisterAttached("ColumnWidth", typeof(GridLength), typeof(AutoGrid), new FrameworkPropertyMetadata(GridLength.Auto, FrameworkPropertyMetadataOptions.AffectsMeasure, FixedColumnWidthChanged));

        public static readonly DependencyProperty IsAutoIndexingProperty = DependencyProperty.Register("IsAutoIndexing", typeof(bool), typeof(AutoGrid), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AutoGrid), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RowCountProperty = DependencyProperty.RegisterAttached("RowCount", typeof(int), typeof(AutoGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure, RowCountChanged));

        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.RegisterAttached("RowHeight", typeof(GridLength), typeof(AutoGrid), new FrameworkPropertyMetadata(GridLength.Auto, FrameworkPropertyMetadataOptions.AffectsMeasure, FixedRowHeightChanged));

        public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached("Rows", typeof(string), typeof(AutoGrid), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure, RowsChanged));

        [Category("Layout")]
        [Description("Presets the horizontal alignment of all child controls")]
        public HorizontalAlignment? ChildHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment?)GetValue(ChildHorizontalAlignmentProperty);
            }
            set
            {
                SetValue(ChildHorizontalAlignmentProperty, value);
            }
        }

        [Category("Layout")]
        [Description("Presets the margin of all child controls")]
        public Thickness? ChildMargin
        {
            get
            {
                return (Thickness?)GetValue(ChildMarginProperty);
            }
            set
            {
                SetValue(ChildMarginProperty, value);
            }
        }

        [Description("Presets the vertical alignment of all child controls")]
        [Category("Layout")]
        public VerticalAlignment? ChildVerticalAlignment
        {
            get
            {
                return (VerticalAlignment?)GetValue(ChildVerticalAlignmentProperty);
            }
            set
            {
                SetValue(ChildVerticalAlignmentProperty, value);
            }
        }

        [Category("Layout")]
        [Description("Defines a set number of columns")]
        public int ColumnCount
        {
            get
            {
                return (int)GetValue(ColumnCountProperty);
            }
            set
            {
                SetValue(ColumnCountProperty, value);
            }
        }

        [Description("Defines all columns using comma separated grid length notation")]
        [Category("Layout")]
        public string Columns
        {
            get
            {
                return (string)GetValue(ColumnsProperty);
            }
            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        [Description("Presets the width of all columns set using the ColumnCount property")]
        [Category("Layout")]
        public GridLength ColumnWidth
        {
            get
            {
                return (GridLength)GetValue(ColumnWidthProperty);
            }
            set
            {
                SetValue(ColumnWidthProperty, value);
            }
        }

        [Category("Layout")]
        [Description("Set to false to disable the auto layout functionality")]
        public bool IsAutoIndexing
        {
            get
            {
                return (bool)GetValue(IsAutoIndexingProperty);
            }
            set
            {
                SetValue(IsAutoIndexingProperty, value);
            }
        }

        [Category("Layout")]
        [Description("Defines the directionality of the autolayout. Use vertical for a column first layout, horizontal for a row first layout.")]
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        [Category("Layout")]
        [Description("Defines a set number of rows")]
        public int RowCount
        {
            get
            {
                return (int)GetValue(RowCountProperty);
            }
            set
            {
                SetValue(RowCountProperty, value);
            }
        }

        [Description("Presets the height of all rows set using the RowCount property")]
        [Category("Layout")]
        public GridLength RowHeight
        {
            get
            {
                return (GridLength)GetValue(RowHeightProperty);
            }
            set
            {
                SetValue(RowHeightProperty, value);
            }
        }

        [Description("Defines all rows using comma separated grid length notation")]
        [Category("Layout")]
        public string Rows
        {
            get
            {
                return (string)GetValue(RowsProperty);
            }
            set
            {
                SetValue(RowsProperty, value);
            }
        }

        public static void ColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue >= 0)
            {
                AutoGrid autoGrid = d as AutoGrid;
                GridLength width = GridLength.Auto;
                if (autoGrid.ColumnDefinitions.Count > 0)
                {
                    width = autoGrid.ColumnDefinitions[0].Width;
                }

                autoGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < (int)e.NewValue; i++)
                {
                    autoGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = width
                    });
                }
            }
        }

        public static void ColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!((string)e.NewValue == string.Empty))
            {
                AutoGrid autoGrid = d as AutoGrid;
                autoGrid.ColumnDefinitions.Clear();
                GridLength[] array = Parse((string)e.NewValue);
                GridLength[] array2 = array;
                foreach (GridLength width in array2)
                {
                    autoGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = width
                    });
                }
            }
        }

        public static void FixedColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoGrid? autoGrid = d as AutoGrid;

            if(autoGrid is not null)
            {
                if (autoGrid.ColumnDefinitions.Count == 0)
                {
                    autoGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int i = 0; i < autoGrid.ColumnDefinitions.Count; i++)
                {
                    autoGrid.ColumnDefinitions[i].Width = (GridLength)e.NewValue;
                }
            }         
        }

        public static void FixedRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoGrid? autoGrid = d as AutoGrid;

            if(autoGrid is not null)
            {
                if (autoGrid.RowDefinitions.Count == 0)
                {
                    autoGrid.RowDefinitions.Add(new RowDefinition());
                }

                for (int i = 0; i < autoGrid.RowDefinitions.Count; i++)
                {
                    autoGrid.RowDefinitions[i].Height = (GridLength)e.NewValue;
                }
            }          
        }

        public static GridLength[] Parse(string text)
        {
            string[] array = text.Split(',');
            GridLength[] array2 = new GridLength[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                string text2 = array[i];
                double result;
                if (text2.Contains('*'))
                {
                    if (!double.TryParse(text2.Replace("*", ""), out result))
                    {
                        result = 1.0;
                    }

                    ref GridLength reference = ref array2[i];
                    reference = new GridLength(result, GridUnitType.Star);
                }
                else if (double.TryParse(text2, out result))
                {
                    ref GridLength reference2 = ref array2[i];
                    reference2 = new GridLength(result);
                }
                else
                {
                    ref GridLength reference3 = ref array2[i];
                    reference3 = GridLength.Auto;
                }
            }

            return array2;
        }

        public static void RowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue >= 0)
            {
                AutoGrid? autoGrid = d as AutoGrid;
                if( autoGrid is not null)
                {
                    GridLength height = GridLength.Auto;
                    if (autoGrid.RowDefinitions.Count > 0)
                    {
                        height = autoGrid.RowDefinitions[0].Height;
                    }

                    autoGrid.RowDefinitions.Clear();
                    for (int i = 0; i < (int)e.NewValue; i++)
                    {
                        autoGrid.RowDefinitions.Add(new RowDefinition
                        {
                            Height = height
                        });
                    }
                }               
            }
        }

        public static void RowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!((string)e.NewValue == string.Empty))
            {
                AutoGrid? autoGrid = d as AutoGrid;
                if (autoGrid is not null)
                {
                    autoGrid.RowDefinitions.Clear();
                    GridLength[] array = Parse((string)e.NewValue);
                    GridLength[] array2 = array;
                    foreach (GridLength height in array2)
                    {
                        autoGrid.RowDefinitions.Add(new RowDefinition
                        {
                            Height = height
                        });
                    }
                }                
            }
        }

        private static void OnChildHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoGrid? autoGrid = d as AutoGrid;
            if (autoGrid is not null)
            {
                foreach (UIElement child in autoGrid.Children)
                {
                    if (autoGrid.ChildHorizontalAlignment.HasValue)
                    {
                        child.SetValue(FrameworkElement.HorizontalAlignmentProperty, autoGrid.ChildHorizontalAlignment);
                    }
                    else
                    {
                        child.SetValue(FrameworkElement.HorizontalAlignmentProperty, DependencyProperty.UnsetValue);
                    }
                }
            }           
        }

        private static void OnChildMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoGrid? autoGrid = d as AutoGrid;
            if (autoGrid is not null)
            {
                foreach (UIElement child in autoGrid.Children)
                {
                    if (autoGrid.ChildMargin.HasValue)
                    {
                        child.SetValue(FrameworkElement.MarginProperty, autoGrid.ChildMargin);
                    }
                    else
                    {
                        child.SetValue(FrameworkElement.MarginProperty, DependencyProperty.UnsetValue);
                    }
                }
            }
          
        }

        private static void OnChildVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoGrid? autoGrid = d as AutoGrid;
            if (autoGrid is not null)
            {
                foreach (UIElement child in autoGrid.Children)
                {
                    if (autoGrid.ChildVerticalAlignment.HasValue)
                    {
                        child.SetValue(FrameworkElement.VerticalAlignmentProperty, autoGrid.ChildVerticalAlignment);
                    }
                    else
                    {
                        child.SetValue(FrameworkElement.VerticalAlignmentProperty, DependencyProperty.UnsetValue);
                    }
                }
            }
      
        }

        private void ApplyChildLayout(UIElement child)
        {
            if (ChildMargin.HasValue)
            {
                child.SetIfDefault(FrameworkElement.MarginProperty, ChildMargin.Value);
            }

            if (ChildHorizontalAlignment.HasValue)
            {
                child.SetIfDefault(FrameworkElement.HorizontalAlignmentProperty, ChildHorizontalAlignment.Value);
            }

            if (ChildVerticalAlignment.HasValue)
            {
                child.SetIfDefault(FrameworkElement.VerticalAlignmentProperty, ChildVerticalAlignment.Value);
            }
        }

        private int Clamp(int value, int max)
        {
            return (value > max) ? max : value;
        }

        private void PerformLayout()
        {
            bool flag = Orientation == Orientation.Horizontal;
            int count = base.RowDefinitions.Count;
            int count2 = base.ColumnDefinitions.Count;
            if (count == 0 || count2 == 0)
            {
                return;
            }

            int num = 0;
            bool[,] array = new bool[count, count2];
            foreach (UIElement child in base.Children)
            {
                bool flag2 = child.Visibility == Visibility.Collapsed;
                if (IsAutoIndexing && !flag2)
                {
                    if (flag)
                    {
                        int num2 = Clamp(num / count2, count - 1);
                        int num3 = Clamp(num % count2, count2 - 1);
                        if (array[num2, num3])
                        {
                            num++;
                            num2 = num / count2;
                            num3 = num % count2;
                        }

                        Grid.SetRow(child, num2);
                        Grid.SetColumn(child, num3);
                        num += Grid.GetColumnSpan(child);
                        int num4 = Grid.GetRowSpan(child) - 1;
                        while (num4 > 0)
                        {
                            array[num2 + num4--, num3] = true;
                        }
                    }
                    else
                    {
                        int num2 = Clamp(num % count, count - 1);
                        int num3 = Clamp(num / count, count2 - 1);
                        if (array[num2, num3])
                        {
                            num++;
                            num2 = num % count;
                            num3 = num / count;
                        }

                        Grid.SetRow(child, num2);
                        Grid.SetColumn(child, num3);
                        num += Grid.GetRowSpan(child);
                        int num4 = Grid.GetColumnSpan(child) - 1;
                        while (num4 > 0)
                        {
                            array[num2, num3 + num4--] = true;
                        }
                    }
                }

                ApplyChildLayout(child);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            PerformLayout();
            return base.MeasureOverride(constraint);
        }
    }
    public static class DependencyExtensions
    {
        public static bool SetIfDefault<T>(this DependencyObject o, DependencyProperty property, T value)
        {
            if (DependencyPropertyHelper.GetValueSource(o, property).BaseValueSource == BaseValueSource.Default)
            {
                o.SetValue(property, value);
                return true;
            }

            return false;
        }
    }
}
