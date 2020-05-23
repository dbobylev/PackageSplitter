using OracleParser.Model;
using PackageSplitter.Model;
using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PackageSplitter.ViewModel
{
    public class SplitterPackageViewModel : PropertyChangedBase
    {
        SplitterPackage _model;
        List<SplitterPackageElementViewModel> Elements = new List<SplitterPackageElementViewModel>();

        public SplitterPackageViewModel(SplitterPackage model)
        {
            _model = model;
            for (int i = 0; i < model.Elements.Count; i++)
            {
                Elements.Add(new SplitterPackageElementViewModel(model.Elements[i]));
            }
        }
    }
}
