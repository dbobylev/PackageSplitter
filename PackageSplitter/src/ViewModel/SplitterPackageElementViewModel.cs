using OracleParser.Model;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.ViewModel
{
    public class SplitterPackageElementViewModel : PropertyChangedBase
    {
        private SplitterPackageElement _model;

        public SplitterPackageElementViewModel(SplitterPackageElement model)
        {
            _model = model;
        }

        public string Name => _model.PackageElementName;
        public ePackageElementType ElementType => _model.PackageElementType;
        public eElementStateType OldSpecState { get => _model.OldSpec; set { _model.OldSpec = value; OnPropertyChanged(); } }
        public eElementStateType OldBodyState { get => _model.OldBody; set { _model.OldBody = value; OnPropertyChanged(); } }
        public eElementStateType NewSpecState { get => _model.NewSpec; set { _model.NewSpec = value; OnPropertyChanged(); } }
        public eElementStateType NewBodyState { get => _model.NewBody; set { _model.NewBody = value; OnPropertyChanged(); } }

        public void UpdateStates(Dictionary<eSplitterObjectType, eElementStateType> newStates)
        {
            foreach(var key in newStates.Keys)
                switch (key)
                {
                    case eSplitterObjectType.OldSpec: OldSpecState = newStates[key]; break;
                    case eSplitterObjectType.OldBody: OldBodyState = newStates[key]; break;
                    case eSplitterObjectType.NewSpec: NewSpecState = newStates[key]; break;
                    case eSplitterObjectType.NewBody: NewBodyState = newStates[key]; break;
                    default: break;
                }
        }
    }
}
