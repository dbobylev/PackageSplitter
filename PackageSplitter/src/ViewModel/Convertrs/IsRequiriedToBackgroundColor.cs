using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace PackageSplitter.ViewModel.Convertrs
{
    class IsRequiriedToBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            if (val)
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            else
                return new SolidColorBrush(Color.FromRgb(0, 0, 255));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
