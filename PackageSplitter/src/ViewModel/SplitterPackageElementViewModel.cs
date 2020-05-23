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
        SplitterPackageElement _model;

        public SplitterPackageElementViewModel(SplitterPackageElement model)
        {
            _model = model;
        }

        public string Name => _model.PackageElementName;
    }
}
