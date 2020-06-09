using DataBaseRepository.Model;
using PackageSplitter.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model
{
    static class Helper
    {
        public static eRepositoryObjectType GetRepositoryType(this eSplitterObjectType source)
        {
            switch (source)
            {
                case eSplitterObjectType.OldSpec:
                case eSplitterObjectType.NewSpec:
                    return eRepositoryObjectType.Package_Spec;
                case eSplitterObjectType.OldBody:
                case eSplitterObjectType.NewBody:
                    return eRepositoryObjectType.Package_Body;
                case eSplitterObjectType.None:
                    return eRepositoryObjectType.None;
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool IsNew(this eSplitterObjectType source)
        {
            return source == eSplitterObjectType.NewBody || source == eSplitterObjectType.NewSpec;
        }

        public static T[] Insert<T>(this T[] source, int pos, T[] value)
        {
            var len = value.Length;
            Array.Resize(ref source, source.Length + len);
            for (int i = source.Length - 1; i >= pos; i--)
            {
                if (i >= pos + len)
                    source[i] = source[i - len];
                else
                    source[i] = value[i - pos];
            }
            return source;
        }
    }
}
