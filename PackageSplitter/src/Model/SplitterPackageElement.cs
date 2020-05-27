using OracleParser.Model;
using OracleParser.Model.PackageModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PackageSplitter.Model
{
    public class SplitterPackageElement
    {
        public ePackageElementType PackageElementType { get; set; }

        public string PackageElementName { get; private set; }

        private eElementStateType _OldSpec;
        public eElementStateType OldSpec
        {
            get => _OldSpec; 
            set => _OldSpec = SetState(_OldSpec, value);
        }

        private eElementStateType _OldBody;
        public eElementStateType OldBody
        {
            get => _OldBody;
            set => _OldBody = SetState(_OldBody, value);
        }

        private eElementStateType _NewSpec;
        public eElementStateType NewSpec
        {
            get => _NewSpec;
            set => _NewSpec = SetState(_NewSpec, value);
        }

        private eElementStateType _NewBody;
        public eElementStateType NewBody
        {
            get => _NewBody;
            set => _NewBody = SetState(_NewBody, value);
        }

        public SplitterPackageElement(string packageElementName, ePackageElementType packageElementType)
        {
            PackageElementName = packageElementName;
            PackageElementType = packageElementType;
            OldSpec = eElementStateType.Empty;
            OldBody = eElementStateType.Empty;
            NewSpec = eElementStateType.Empty;
            NewBody = eElementStateType.Empty;
        }

        private eElementStateType SetState(eElementStateType currentValue, eElementStateType newValue)
        {
            if (currentValue == eElementStateType.Exist && newValue == eElementStateType.Add)
                return currentValue;
            else if (currentValue == eElementStateType.Delete && newValue == eElementStateType.Add)
                return eElementStateType.Exist;
            else if (currentValue == eElementStateType.Add && newValue == eElementStateType.Delete)
                return eElementStateType.Empty;
            else if (currentValue == eElementStateType.Empty && newValue == eElementStateType.Delete)
                return currentValue;
            else if (currentValue == eElementStateType.CreateLink && newValue == eElementStateType.Delete)
                return eElementStateType.Exist;
            else if (currentValue == eElementStateType.CreateLink)
                return currentValue;
            else
                return newValue;
        }
    }
}
