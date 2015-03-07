using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;


namespace Processes_Manage
{
    public sealed class BackgroundConverter : IValueConverter
    {
        SolidColorBrush lightBlueBrush = new SolidColorBrush(Color.FromArgb(255,240,240,255));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            // Get the index of a ListViewItem
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);

            if (index % 2 == 0)
            {
                return lightBlueBrush;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            // Get the index of a ListViewItem
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);

            if (index % 2 == 0)
            {
                return lightBlueBrush;
            }
            else
            {
                return Brushes.White;
            }
        }
    }
}
