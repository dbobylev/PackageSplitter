using OracleParser.Model;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.src.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace PackageSplitter.ViewModel
{
    public class SplitterPackageViewModel : PropertyChangedBase
    {
        private SplitterPackage _model;
        private eSplitParam _defaultNewObjParam = eSplitParam.CopyToClipBoard | eSplitParam.GenerateHeader | eSplitParam.OpenNewWindow;
        public RelayCommand SplitCommand { get; private set; }

        public ObservableCollection<SplitterPackageElementViewModel> ElementsViewModel { get; private set; }

        public SplitterPackageViewModel()
        {
            ElementsViewModel = new ObservableCollection<SplitterPackageElementViewModel>();
            SplitCommand = new RelayCommand(RunSplit);
        }

        public void SetModel(SplitterPackage model)
        {
            _model = model;
            ElementsViewModel.Clear();

            for (int i = 0; i < _model.Elements.Count; i++)
                ElementsViewModel.Add(new SplitterPackageElementViewModel(_model.Elements[i]));
        }

        private void RunSplit(object param)
        {
            if (param is eSplitterObjectType splitterObjectType)
            {
                SplitManager.Instance().SetSplitterPackage(_model);
                SplitManager.Instance().RunSplit(splitterObjectType, _defaultNewObjParam);
            }
            else
            {
                throw new NotImplementedException("Не подходящий параметр для запуска Split");
            }
        }
    }
}
