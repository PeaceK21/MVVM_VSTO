using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemoList.UI.Controls
{
    public class DataGridEx : DataGrid
    {
        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public bool CanUserFilter
        {
            get { return (bool)GetValue(CanUserFilterProperty); }
            set { SetValue(CanUserFilterProperty, value); }
        }

        public bool FilterIsEnabled
        {
            get { return (bool)GetValue(FilterIsEnabledProperty); }
            set { SetValue(FilterIsEnabledProperty, value); }
        }

        public ICommand RowClickedCommand
        {
            get { return (ICommand)GetValue(RowClickedCommandProperty); }
            set { SetValue(RowClickedCommandProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = 
            DependencyProperty.Register("Filter", typeof(string), typeof(DataGridEx), new PropertyMetadata(FilterPropertyChanged));

        public static readonly DependencyProperty CanUserFilterProperty =
            DependencyProperty.Register("CanUserFilter", typeof(bool), typeof(DataGridEx), new PropertyMetadata(true, CanUserFilterPropertyChanged));

        public static readonly DependencyProperty FilterIsEnabledProperty =
            DependencyProperty.Register("FilterIsEnabled", typeof(bool), typeof(DataGridEx), new PropertyMetadata(true, FilterIsEnabledPropertyChanged));

        public static readonly DependencyProperty RowClickedCommandProperty =
            DependencyProperty.Register("RowClickedCommand", typeof(ICommand), typeof(DataGridEx), new PropertyMetadata(null));

        private static CultureInfo enCultureInfo = CultureInfo.GetCultureInfo("en-US");
        private static Timer refreshTimer;

        internal IDictionary<string, string> ColumnFormats { get; set; }

        static DataGridEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridEx), new FrameworkPropertyMetadata(typeof(DataGridEx)));
            ItemsSourceProperty.OverrideMetadata(typeof(DataGridEx), new FrameworkPropertyMetadata("ItemsSource", ItemsSourcePropertyChanged));
        }

        public DataGridEx()
        {
            SetResourceReference(StyleProperty, typeof(DataGrid));

            MouseUp += MouseUpHandler;
            LayoutUpdated += LayoutUpdatedHandler;

            IsReadOnly = true;
        }

        private void LayoutUpdatedHandler(object sender, EventArgs e)
        {
            PopulateColumnFormats();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            try
            {
                BindFilterTextBox();
            }
            catch
            {
                // Prevent XAML designer error
            }
        }

        private void BindFilterTextBox()
        {
            var filterTextBox = GetTemplateChild("PART_FilterTextBox") as TextBox;
            filterTextBox.Visibility = Visibility.Visible;

            var binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Filter");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            BindingOperations.SetBinding(filterTextBox, TextBox.TextProperty, binding);
        }

        private static void CanUserFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGridEx;
            var filter = dataGrid.GetTemplateChild("PART_Filter") as Border;

            filter.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void FilterIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGridEx;
            var filterTextBox = dataGrid.GetTemplateChild("PART_FilterTextBox") as TextBox;

            filterTextBox.IsEnabled = (bool)e.NewValue;
        }

        private static void FilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGridEx;

            if (dataGrid.ItemsSource == null)
                return;

            if (dataGrid.ItemsSource is ListCollectionView itemsSource)
            {
                // CommitEdit should be 2 times
                dataGrid.CommitEdit();
                dataGrid.CommitEdit();

                // Don't filter whenever a user inputs a character. Wait until the end
                if (refreshTimer != null)
                {
                    refreshTimer.Change(500, Timeout.Infinite);
                }
                else
                {
                    refreshTimer = new Timer((state) =>
                    {
                        dataGrid.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            itemsSource.Refresh();
                            refreshTimer.Dispose();
                            refreshTimer = null;
                        }));
                    });

                    refreshTimer.Change(500, Timeout.Infinite);
                }
            }
        }

        internal void PopulateColumnFormats()
        {
            if (ColumnFormats != null)
                return;

            if (Items == null || Items.Count == 0)
                return;

            ColumnFormats = new Dictionary<string, string>();

            var templateColumns = Columns
                .OfType<DataGridTemplateColumn>()
                .ToList();

            foreach (var column in templateColumns)
            {
                var contentPresenter = column.GetCellContent(Items[0]);

                if (contentPresenter == null)
                    continue;

                var textBox = GetVisualChild<DataGridExTextBlock>(contentPresenter);

                if (textBox != null)
                {
                    var bindingExpression = textBox.GetBindingExpression(DataGridExTextBlock.TextProperty);
                    var parentBinding = bindingExpression.ParentBinding;

                    ColumnFormats.Add(parentBinding.Path.Path, parentBinding.StringFormat);
                }
            }

            if (ColumnFormats.Count == 0)
                ColumnFormats = null;
        }

        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGridEx;
            var items = e.NewValue as ListCollectionView;

            dataGrid.ColumnFormats = null;

            if (items == null)
                return;

            var properties = items.ItemProperties
                .Select(p => p.Descriptor as PropertyDescriptor)
                .ToList();

            items.Filter = o =>
            {
                if (string.IsNullOrEmpty(dataGrid.Filter))
                    return true;

                var filter = dataGrid.Filter.ToLower();

                foreach (var property in properties)
                {
                    if (!dataGrid.ColumnFormats.ContainsKey(property.Name))
                        continue;

                    var value = property.GetValue(o);

                    if (value == null)
                        continue;

                    var valueString = string.Empty;
                    var format = dataGrid.ColumnFormats[property.Name];

                    if (value is IFormattable formattable)
                        valueString = formattable.ToString(format, enCultureInfo);
                    else
                        valueString = value.ToString().ToLower();

                    if (valueString.Contains(filter))
                        return true;
                }

                return false;
            };
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            var child = default(T);
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            
            for (int i = 0; i < childrenCount; i++)
            {
                var visual = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = visual as T;
                
                if (child == null)
                    child = GetVisualChild<T>(visual);
                
                if (child != null)
                    break;
            }
            
            return child;
        }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            var row = FindParentRow(e.OriginalSource as DependencyObject);

            if (row != null && RowClickedCommand != null)
                RowClickedCommand.Execute(row);
        }

        private DependencyObject FindParentRow(DependencyObject control)
        {
            while (control != null)
            {
                if (control is DataGridRow)
                    break;

                control = VisualTreeHelper.GetParent(control);
            }

            return control;
        }
    }
}