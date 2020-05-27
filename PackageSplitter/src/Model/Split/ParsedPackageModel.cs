using OracleParser.Model.PackageModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model.Split
{
    public class ParsedPackageModel
    {
        public Package Package { get; private set; }

        private static ParsedPackageModel _instance;
        public static ParsedPackageModel Instance()
        {
            if (_instance == null)
                _instance = new ParsedPackageModel();

            return _instance;
        }

        private ParsedPackageModel()
        {

        }

        public void SetPackage(Package package)
        {
            Package = package;
        }
    }
}
