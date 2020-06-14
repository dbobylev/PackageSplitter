using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace PackageSplitter.ViewModel.Convertrs
{
    static class Helper
    {
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes?.ToArray()[0];
        }

        public static string GetDescription(this Enum value)
        {
            var attribute = value.GetAttributeOfType<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static T FindResource<T>(this string source)
        {
            return (T)Application.Current.FindResource(source);
        }

    }
}
