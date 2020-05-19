using PackageSplitter.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace PackageSplitter.ViewModel.Convertrs
{
    class ElementStateToStackPanelStyle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            eElementStateType state = (eElementStateType)value;

            Style answer = null;

            switch (state)
            {
                case eElementStateType.Exist:
                case eElementStateType.Empty:
                    answer = Application.Current.FindResource("CellStackPanelDefaultStyle") as Style;
                    break;
                case eElementStateType.Add:
                    answer = Application.Current.FindResource("CellStackPanelAddStyle") as Style;
                    break;
                case eElementStateType.Delete:
                    answer = Application.Current.FindResource("CellStackPanelDeleteStyle") as Style;
                    break;
                case eElementStateType.CreateLink:
                    answer = Application.Current.FindResource("CellStackPanelLinkStyle") as Style;
                    break;
                default:
                    break;
            }

            return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
