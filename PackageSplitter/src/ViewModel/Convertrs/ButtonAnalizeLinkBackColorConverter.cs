using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PackageSplitter.ViewModel.Convertrs
{
    class ButtonAnalizeLinkBackColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush answer = "cUI".FindResource<SolidColorBrush>();
            //bool? val = (bool?)value;
            if ((bool)value)
                answer = "cCellYellow".FindResource<SolidColorBrush>();
            return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
