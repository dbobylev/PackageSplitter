using DataBaseRepository.Model;
using OracleParser.Model.PackageModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model.Split
{
    public interface ISplitManager
    {
        event Action<Package> OraclePackageSetted;
        void SetSplitterPackage(Splitter splitterPackage);
        void RunSplit(eSplitterObjectType splitterObjectType, eSplitParam param);
        bool AnalizeLinks();
    }
}
