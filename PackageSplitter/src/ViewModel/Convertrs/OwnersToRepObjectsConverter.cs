using DataBaseRepository;
using DataBaseRepository.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PackageSplitter.ViewModel.Convertrs
{
    class OwnersToRepObjectsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || string.IsNullOrEmpty((string)values[0]))
                return new RepositoryObject[] { };

            string Owner = (string)values[0];
            string Pattern = (string)values[1];

            SelectRequest request = new SelectRequest() { Owner = Owner, Pattern = Pattern, FileTypes = new List<eRepositoryObjectType>() { eRepositoryObjectType.Package_Body } };
            RepositoryObject[] objects = DBRep.Instance().GetFiles(request).ToArray();

            return objects;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
