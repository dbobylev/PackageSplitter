using DataBaseRepository.Model;
using OracleParser;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.Tests.Split.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Tests.Split.Cases
{
    class SplitPackageCaseBase
    {
        private IOraParser _OraParser;
        private SplitManager _SplitManager; 

        protected Package _package;
        protected Splitter _splitter;

        public string ExceptedOldSpec { get; protected set; }
        public string ExceptedOldBody { get; protected set; }
        public string ExceptedNewSpec { get; protected set; }
        public string ExceptedNewBody { get; protected set; }

        public SplitPackageCaseBase(PackageText packageText)
        {
            var repositoryPackage = packageText.RepositoryPackage;

            _OraParser = OraParser.Instance();

            _package = _OraParser.ParsePackage(repositoryPackage, false).Result;
            _splitter = new Splitter(_package);

            _SplitManager = SplitManager.Instance();
            _SplitManager.SetOracleParsedPackage(_package);
            _SplitManager.SetSplitterPackage(_splitter);
        }

        public void RunSplit()
        {

        }

    }
}
