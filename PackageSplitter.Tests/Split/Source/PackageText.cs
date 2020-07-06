using DataBaseRepository.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PackageSplitter.Tests.Split.Source
{
    public abstract class PackageText
    {
        public RepositoryPackage RepositoryPackage { get; private set; }
        
        public string PackageName { get; protected set; }

        public string SpecText { get; protected set; }
                              
        public string BodyText { get; protected set; }

        public PackageText()
        {
            
        }

        protected void SaveTextToFile()
        {
            RepositoryPackage = new RepositoryPackage(PackageName, "TestRepository");

            if (!Directory.Exists(RepositoryPackage.OwnerFullPath))
                Directory.CreateDirectory(RepositoryPackage.OwnerFullPath);

            File.WriteAllText(RepositoryPackage.SpecRepFullPath, SpecText);
            File.WriteAllText(RepositoryPackage.BodyRepFullPath, BodyText);
        }
    }
}
