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

        public RepositoryPackage RepositoryPackage { get; private set; }

        public Dictionary<eSplitterObjectType, string> ExceptedPart { get; private set; }

        public SplitPackageCaseBase(PackageText packageText)
        {
            RepositoryPackage = packageText.RepositoryPackage;
            ExceptedPart = new Dictionary<eSplitterObjectType, string>();

            _OraParser = OraParser.Instance();

            _package = _OraParser.ParsePackage(RepositoryPackage, false).Result;
            _splitter = new Splitter(_package);

            _SplitManager = SplitManager.Instance();
            _SplitManager.SetOracleParsedPackage(_package);
            _SplitManager.SetSplitterPackage(_splitter);
        }

        public void RunSplit(eSplitterObjectType splitterOvjectType, eSplitParam splitterParam)
        {
            // Убираем параметр "Открыть в новом окне" (при его наличии)
            if (splitterParam.HasFlag(eSplitParam.OpenNewWindow))
                splitterParam &= ~eSplitParam.OpenNewWindow;

            _SplitManager.RunSplit(splitterOvjectType, splitterParam);
        }

    }
}
