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
        private SplitterPackage _model;
        public ObservableCollection<SplitterPackageElementViewModel> ElementsViewModel { get; private set; }

        public SplitterPackageViewModel(SplitterPackage model)
        {
            _model = model;
            ElementsViewModel = new ObservableCollection<SplitterPackageElementViewModel>();

            for (int i = 0; i < model.Elements.Count; i++)
                ElementsViewModel.Add(new SplitterPackageElementViewModel(model.Elements[i]));
        }
    }
}
