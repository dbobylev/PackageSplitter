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
        public eElementStateType OldSpec { get => _model.OldSpec; set { _model.OldSpec = value; OnPropertyChanged(); } }
        public eElementStateType OldBody { get => _model.OldBody; set { _model.OldBody = value; OnPropertyChanged(); } }
        public eElementStateType NewSpec { get => _model.NewSpec; set { _model.NewSpec = value; OnPropertyChanged(); } }
        public eElementStateType NewBody { get => _model.NewBody; set { _model.NewBody = value; OnPropertyChanged(); } }
    }
}
