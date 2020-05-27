using OracleParser.Model.PackageModel;
using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model.Split
{
    public class SplitManager
    {
        private Package _package;
        private SplitterPackage _splitterPackage;

        private static SplitManager _instance;
        public static SplitManager Instance()
        {
            if (_instance == null)
                _instance = new SplitManager();
            return _instance;
        }
        private SplitManager()
        {

        }

        public void SetParsedPackage(Package package)
        {
            _package = package;
        }

        public void SetSplitterPackage(SplitterPackage splitterPackage)
        {
            _splitterPackage = splitterPackage;
        }

        public void RunSplit(eSplitterObjectType objectType, eSplitParam param)
        {

        }
    }
}
