using DataBaseRepository.Model;
using OracleParser.Model.PackageModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model.Split
{
    public interface ISplitManager
    {
        event Action<Package> PackageLoaded;
        void LoadOracleParsedPackage(RepositoryPackage repositoryPackage);
        void LoadSplitterPackage(Splitter splitterPackage);
        void RunSplit(eSplitterObjectType splitterObjectType, eSplitParam param);
        void AnalizeLinks();
    }
}
