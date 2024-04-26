using System.Windows;
using System.Windows.Controls;

namespace CoreMvvmLib.Component
{
    public class CustomAutoGrid : Grid
    {
        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }
        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }
        public string Columns
        {
            get { return (string)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        public string Rows
        {
            get { return (string)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(string), typeof(CustomAutoGrid), new PropertyMetadata("", new PropertyChangedCallback(RowsChanged)));
        // Using a DependencyProperty as the backing store for ColumnsProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(string), typeof(CustomAutoGrid), new PropertyMetadata("", new PropertyChangedCallback(ColumnsChanged)));
        // Using a DependencyProperty as the backing store for RowCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(CustomAutoGrid), new PropertyMetadata(1, new PropertyChangedCallback(RowCountChanged)));

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
                    DependencyProperty.Register("ColumnCount", typeof(int), typeof(CustomAutoGrid), new PropertyMetadata(1, new PropertyChangedCallback(ColumnCountChanged)));
        private static void RowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((string)e.NewValue == string.Empty)
                return;

            var grid = d as CustomAutoGrid;
            grid.RowDefinitions.Clear();

            var defs = Parse((string)e.NewValue);
            foreach (var def in defs)
                grid.RowDefinitions.Add(new RowDefinition() { Height = def });
        }
        private static void ColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((string)e.NewValue == string.Empty)
                return;

            var grid = d as CustomAutoGrid;
            grid.ColumnDefinitions.Clear();

            var defs = Parse((string)e.NewValue);
            foreach (var def in defs)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = def });
        }
        private static void RowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue < 0) return;
            var grid = d as CustomAutoGrid;
            var height = GridLength.Auto;
            if (grid.RowDefinitions.Count > 0)
                height = grid.RowDefinitions[0].Height;

            grid.RowDefinitions.Clear();
            for (int i = 0; i < (int)e.NewValue; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = height });

        }
        private static void ColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue < 0)
                return;

            var grid = d as CustomAutoGrid;

            // look for an existing column definition for the height
            var width = GridLength.Auto;
            if (grid.ColumnDefinitions.Count > 0)
                width = grid.ColumnDefinitions[0].Width;

            // clear and rebuild
            grid.ColumnDefinitions.Clear();
            for (int i = 0; i < (int)e.NewValue; i++)
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() { Width = width });
        }
        private static GridLength[] Parse(string text)
        {
            var tokens = text.Split(',');
            var definitions = new GridLength[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                var str = tokens[i];
                double value;
                if (str.Contains('*'))
                {
                    if (!double.TryParse(str.Replace("*", ""), out value))
                        value = 1.0;
                    definitions[i] = new GridLength(value, GridUnitType.Star);
                    continue;
                }
                if (double.TryParse(str, out value))
                {
                    definitions[i] = new GridLength(value);
                    continue;
                }
                definitions[i] = GridLength.Auto;
            }
            return definitions;
        }
        private void PerformLayout()
        {
            var rowCount = this.RowDefinitions.Count;
            var colCount = this.ColumnDefinitions.Count;
            if (rowCount == 0 || colCount == 0) return;

            var position = 0;
            var skip = new bool[rowCount, colCount];
            foreach (UIElement child in Children)
            {
                var isChildIsCollapsed = child.Visibility == Visibility.Collapsed;
                if (isChildIsCollapsed)
                {
                    var row = Clamp(position / colCount, rowCount - 1);
                    var col = Clamp(position % colCount, colCount - 1);
                    if (skip[row, col])
                    {
                        position++;
                        row = (position / colCount);
                        col = (position % colCount);
                    }
                    Grid.SetRow(child, row);
                    Grid.SetColumn(child, col);
                    position += Grid.GetColumnSpan(child);

                    var offset = Grid.GetRowSpan(child) - 1;
                    while (offset > 0)
                    {
                        skip[row + offset--, col] = true;
                    }
                }
            }

        }



        private int Clamp(int value, int max)
        {
            return (value > max) ? max : value;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.PerformLayout();
            return base.MeasureOverride(constraint);
        }
    }
}
