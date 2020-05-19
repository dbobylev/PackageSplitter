using OracleParser.Model;
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
        private List<SplitterPackageElement> _elements = new List<SplitterPackageElement>();

        public IReadOnlyCollection<SplitterPackageElement> Elements { get => _elements.AsReadOnly(); }

        public SplitterPackage(ParsedPackage package)
        {
            for (int i = 0; i < package.Body.Procedures.Count; i++)
            {
                string elementName = package.Body.Procedures[i].Name;

                var element = new SplitterPackageElement(elementName);
                element.SetOldBody(package.Body.Procedures[i]);

                var SpecElement = package.Spec.Procedures.FirstOrDefault(x => x.Name == elementName);
                if (SpecElement != null)
                    element.SetOldSpec(SpecElement);

                _elements.Add(element);
            }
        }
    }
}
