using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MemoList.UI.Controls
{
    class DataGridExTextBlock : TextBlock
    {
        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        public new static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
            typeof(DataGridExTextBlock), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(UpdateHighlighting)));

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string),
            typeof(DataGridExTextBlock), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(UpdateHighlighting)));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register("HighlightBrush", typeof(Brush),
            typeof(DataGridExTextBlock), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(UpdateHighlighting)));

        public DataGridExTextBlock()
        {
            IsHitTestVisible = false;
        }

        private static void UpdateHighlighting(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplyHighlight(d as DataGridExTextBlock);
        }

        private static void ApplyHighlight(DataGridExTextBlock textBlock)
        {
            var filter = textBlock.Filter;
            var text = textBlock.Text;

            if (string.IsNullOrEmpty(text))
                return;

            if (string.IsNullOrEmpty(filter))
            {
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(text);
            }
            else
            {
                var index = text.IndexOf(filter, StringComparison.OrdinalIgnoreCase);

                textBlock.Inlines.Clear();

                if (index < 0)
                {
                    textBlock.Inlines.Add(text);
                }
                else
                {
                    if (index > 0)
                        textBlock.Inlines.Add(text.Substring(0, index));

                    var run = new Run(text.Substring(index, filter.Length))
                    {
                        Foreground = textBlock.HighlightBrush,
                        FontWeight = FontWeights.Bold
                    };

                    textBlock.Inlines.Add(run);

                    index += filter.Length;

                    if (index < text.Length)
                        textBlock.Inlines.Add(text.Substring(index));
                }
            }
        }
    }
}
