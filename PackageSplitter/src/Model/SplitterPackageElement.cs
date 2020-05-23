using OracleParser.Model;
using OracleParser.Model.PackageModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model
{
    public class SplitterPackageElement
    {
        public ePackageElementType PackageElementType { get; set; }

        public string PackageElementName { get; private set; }

        public eElementStateType OldSpec { get; set; }
        public eElementStateType OldBody { get; set; }
        public eElementStateType NewSpec { get; set; }
        public eElementStateType NewBody { get; set; }

        public SplitterPackageElement(string packageElementName, ePackageElementType packageElementType)
        {
            PackageElementName = packageElementName;
            PackageElementType = packageElementType;
            OldSpec = eElementStateType.Empty;
            OldBody = eElementStateType.Empty;
            NewSpec = eElementStateType.Empty;
            NewBody = eElementStateType.Empty;
        }
    }
}
