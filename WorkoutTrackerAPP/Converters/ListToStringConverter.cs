using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WorkoutTrackerAPP.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<string> list)
            {
                if (list.Count == 0)
                    return "None";

                var joined = string.Join(", ", list);
                return char.ToUpper(joined[0]) + joined.Substring(1).ToLower();
            }

            if (value is string item && !string.IsNullOrEmpty(item))
                return char.ToUpper(item[0]) + item.Substring(1).ToLower();

            return "None";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
