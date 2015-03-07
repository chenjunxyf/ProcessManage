using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Processes_Manage
{
    class MemoryConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string date = value.ToString();
            return date.ToString() +" "+ "K";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToString();
            int resultType;
            if (int.TryParse(strValue, out resultType))
            {
                return resultType;
            }
            return value;
        }
    }
}
