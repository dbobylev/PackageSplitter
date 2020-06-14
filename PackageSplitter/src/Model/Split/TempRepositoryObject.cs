﻿using DataBaseRepository.Model;
using OracleParser.Model.PackageModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PackageSplitter.Model.Split
{
    public class TempRepositoryObject
    {
        private Func<string, string> TempName = (x) => $"{x}.temp";

        public RepositoryPackage OriginalRepObject { get; private set; }
        public RepositoryPackage TempRepObject { get; private set; }

        public TempRepositoryObject(Package package)
        {
            OriginalRepObject = new RepositoryPackage(package.repositoryPackage.Name, package.repositoryPackage.Owner);
            TempRepObject = new RepositoryPackage(TempName(OriginalRepObject.Name), OriginalRepObject.Owner);
        }

        public void DeleteTempFile()
        {
            Action<string> deleteFile = (x) => { if (File.Exists(x)) File.Delete(x); };
            deleteFile(TempRepObject.BodyRepFullPath);
            deleteFile(TempRepObject.SpecRepFullPath);
        }
    }
}