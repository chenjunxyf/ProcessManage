using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Data;

namespace Processes_Manage
{
    class MethodInvoker: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return typeof(SystemInfo).GetMethod((string)value).Invoke(null, null);
            }
            catch (Exception ex)
            {
                return String.Format("错误：{0}", ex.Message);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
