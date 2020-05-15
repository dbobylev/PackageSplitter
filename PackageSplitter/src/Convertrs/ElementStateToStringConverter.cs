using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace PackageSplitter.Convertrs
{
    class ElementStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var EnumElement = (Enum)value;
            
            if (EnumElement == null)
                throw new NotImplementedException();

            string description = EnumElement.GetDescription();

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
