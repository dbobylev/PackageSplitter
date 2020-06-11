using DataBaseRepository.Model;
using Newtonsoft.Json;
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
    public class Splitter
    {
        [JsonProperty]
        public List<SplitterElement> Elements { get; set; }
        [JsonProperty]
        public RepositoryPackage RepositoryPackage { get; private set; }

        public Splitter(Package package, RepositoryPackage repositoryPackage)
        {
            RepositoryPackage = repositoryPackage;
            Elements = new List<SplitterElement>();

            for (int i = 0; i < package.elements.Count; i++)
            {
                var packageElement = package.elements[i];
                var SplitterElement = new SplitterElement(packageElement.Name, packageElement.ElementType);
                if (packageElement.HasSpec)
                    SplitterElement.OldSpec = eElementStateType.Exist;
                if (packageElement.HasBody)
                    SplitterElement.OldBody = eElementStateType.Exist;
                Elements.Add(SplitterElement);
            }
        }

        public Splitter()
        {

        }
    }
}
