using OracleParser.Model;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageSplitter.Model
{
    public class SplitterPackage
    {
        public List<SplitterPackageElement> Elements { get; set; }

        public SplitterPackage(Package package)
        {
            for (int i = 0; i < package.elements.Count; i++)
            {
                var packageElement = package.elements[i];
                var SplitterElement = new SplitterPackageElement(packageElement.Name, packageElement.ElementType);
                if (packageElement.HasSpec)
                    SplitterElement.OldSpec = eElementStateType.Exist;
                if (packageElement.HasBody)
                    SplitterElement.OldBody = eElementStateType.Exist;
                Elements.Add(SplitterElement);
            }
        }
    }
}
