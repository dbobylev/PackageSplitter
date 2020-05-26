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

        private bool _HasSpec = false;
        private eElementStateType _OldSpec;
        public eElementStateType OldSpec
        {
            get => _OldSpec; 
            set
            {
                if (value == eElementStateType.Exist)
                {
                    _HasSpec = true;
                    _OldSpec = value;
                }
                else if (_OldSpec == eElementStateType.Exist && value == eElementStateType.Add && _HasSpec)
                    return;
                else if (_OldSpec == eElementStateType.Delete && value == eElementStateType.Add && _HasSpec)
                    _OldSpec = eElementStateType.Exist;
                else if (_OldSpec == eElementStateType.Add && value == eElementStateType.Delete && !_HasSpec)
                    _OldSpec = eElementStateType.Empty;
                else if (_OldSpec == eElementStateType.Empty && value == eElementStateType.Delete && !_HasSpec)
                    return;
                else
                    _OldSpec = value;
            }
        }
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
