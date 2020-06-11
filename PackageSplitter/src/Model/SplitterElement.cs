using Newtonsoft.Json;
using OracleParser.Model;
using OracleParser.Model.PackageModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PackageSplitter.Model
{
    public class SplitterElement
    {
        [JsonProperty]
        public ePackageElementType PackageElementType { get; set; }
        
        [JsonProperty]
        public string PackageElementName { get; private set; }

        [JsonProperty]
        private eElementStateType _OldSpec;
        [JsonIgnore]
        public eElementStateType OldSpec
        {
            get => _OldSpec; 
            set => _OldSpec = SetState(_OldSpec, value);
        }

        [JsonProperty]
        private eElementStateType _OldBody;
        [JsonIgnore]
        public eElementStateType OldBody
        {
            get => _OldBody;
            set => _OldBody = SetState(_OldBody, value);
        }

        [JsonProperty]
        private eElementStateType _NewSpec;
        [JsonIgnore]
        public eElementStateType NewSpec
        {
            get => _NewSpec;
            set => _NewSpec = SetState(_NewSpec, value);
        }

        [JsonProperty]
        private eElementStateType _NewBody;
        [JsonIgnore]
        public eElementStateType NewBody
        {
            get => _NewBody;
            set => _NewBody = SetState(_NewBody, value);
        }

        public SplitterElement(string packageElementName, ePackageElementType packageElementType)
        {
            PackageElementName = packageElementName;
            PackageElementType = packageElementType;
            OldSpec = eElementStateType.Empty;
            OldBody = eElementStateType.Empty;
            NewSpec = eElementStateType.Empty;
            NewBody = eElementStateType.Empty;
        }

        public SplitterElement()
        {

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
