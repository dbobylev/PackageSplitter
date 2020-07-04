using DataBaseRepository.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Tests.Split.Source
{
    public abstract class PackageText
    {
        public RepositoryPackage RepositoryPackage { get; private set; }

        private string _PackageName;
        public string PackageName { 
            get => _PackageName; 
            protected set
            {
                _PackageName = value;
                RepositoryPackage = new RepositoryPackage(_PackageName, "ALPHA");
            }
        }

        public string SpecText { get; protected set; }
                              
        public string BodyText { get; protected set; }

        public PackageText()
        {
            
        }

    }
}
