using DataBaseRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PackageSplitter.Convertrs
{
    public class RepPathToOwnersConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Seri.Log.Verbose($"RepPathToOwnersConverter begin value:{value.ToString()}");

            if (!(value is string))
                throw new Exception("Wrong value in RepPathToOwnersConverter");

            string RepPath = (string)value;
            if (string.IsNullOrEmpty(RepPath) || !Directory.Exists(RepPath))
                return new string[] { };

            DBRep.Instance().RepositoryPath = RepPath;
            var owners = DBRep.Instance().GetOwners().ToArray();

            return owners;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
