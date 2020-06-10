using DataBaseRepository.Model;
using OracleParser.Model;
using OracleParser.Model.PackageModel;
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
        private ISplitManager _SplitManager;
        private SplitterPackage _model;
        private eSplitParam _defaultNewObjParam = eSplitParam.CopyToClipBoard | eSplitParam.GenerateHeader | eSplitParam.OpenNewWindow;
        public RelayCommand SplitCommand { get; private set; }

        public ObservableCollection<SplitterPackageElementViewModel> ElementsViewModel { get; private set; }

        public SplitterPackageViewModel(ISplitManager splitManager)
        {
            _SplitManager = splitManager;
            _SplitManager.PackageLoaded += SetModel;
            ElementsViewModel = new ObservableCollection<SplitterPackageElementViewModel>();
            SplitCommand = new RelayCommand(RunSplit);
        }

        public void SetModel(Package parsedPackage, RepositoryPackage repositoryPackage)
        {
            _model = new SplitterPackage(parsedPackage, repositoryPackage);
            ElementsViewModel.Clear();

            for (int i = 0; i < _model.Elements.Count; i++)
                ElementsViewModel.Add(new SplitterPackageElementViewModel(_model.Elements[i]));
        }

        private void RunSplit(object param)
        {
            if (param is eSplitterObjectType splitterObjectType)
            {
                _SplitManager.LoadSplitterPackage(_model);
                _SplitManager.RunSplit(splitterObjectType, _defaultNewObjParam);
            }
            else
            {
                throw new NotImplementedException("Не подходящий параметр для запуска Split");
            }
        }
    }
}
