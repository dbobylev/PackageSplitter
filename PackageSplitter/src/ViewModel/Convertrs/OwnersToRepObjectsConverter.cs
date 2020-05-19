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
    class OwnersToRepObjectsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Seri.Log.Verbose($"OwnersToRepObjectsConverter begin");
            
            if (value == null || string.IsNullOrEmpty((string)value))
                return new RepositoryObject[] { };

            if (!(value is string))
                throw new Exception("Wrong value in OwnersToRepObjectsConverter");


            string Owner = (string)value;

            SelectRequest request = new SelectRequest() { Owner = Owner, FileTypes = new List<eRepositoryObjectType>() { eRepositoryObjectType.Package_Body } };
            RepositoryObject[] objects = DBRep.Instance().GetFiles(request).ToArray();

            Seri.Log.Verbose($"OwnersToRepObjectsConverter find {objects.Count()} objects");

            return objects;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
