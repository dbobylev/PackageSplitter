using PackageSplitter.Model;
using PackageSplitter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageSplitter.Model
{
    class Package
    {
        private List<PackageElement> _elements = new List<PackageElement>();

        public IReadOnlyCollection<PackageElement> Elements { get => _elements.AsReadOnly(); }

        public Package(OracleParser.Model.Package package)
        {
            for (int i = 0; i < package.Body.Procedures.Count; i++)
            {
                string elementName = package.Body.Procedures[i].Name;

                var element = new PackageElement(elementName);
                element.SetOldBody(package.Body.Procedures[i]);

                var SpecElement = package.Spec.Procedures.FirstOrDefault(x => x.Name == elementName);
                if (SpecElement != null)
                    element.SetOldSpec(SpecElement);

                _elements.Add(element);
            }
        }
    }
}
