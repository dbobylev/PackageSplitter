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
    public class SplitterViewModel : PropertyChangedBase
    {
        private ISplitManager _SplitManager;
        private Splitter _model;
        public RelayCommand SplitCommand { get; private set; }

        public bool IsCheckedParamNewWindow { get; set; } = true;
        public bool IsCheckedParamClipboard { get; set; } = true;
        public bool IsCheckedParamAddHeader { get; set; } = true;
        public bool IsCheckedParamUpdateRep { get; set; } = false;

        public ObservableCollection<SplitterElementViewModel> ElementsViewModel { get; private set; }

        public SplitterViewModel(ISplitManager splitManager)
        {
            _SplitManager = splitManager;
            _SplitManager.PackageLoaded += SetModel;
            ElementsViewModel = new ObservableCollection<SplitterElementViewModel>();
            SplitCommand = new RelayCommand(RunSplit);
        }

        public void SetModel(Package parsedPackage, RepositoryPackage repositoryPackage)
        {
            _model = new Splitter(parsedPackage, repositoryPackage);
            ElementsViewModel.Clear();

            for (int i = 0; i < _model.Elements.Count; i++)
                ElementsViewModel.Add(new SplitterElementViewModel(_model.Elements[i]));
        }

        private void RunSplit(object param)
        {
            if (param is eSplitterObjectType splitterObjectType)
            {
                _SplitManager.LoadSplitterPackage(_model);
                _SplitManager.RunSplit(splitterObjectType, GetSplitParam());
            }
            else
            {
                throw new NotImplementedException("Не подходящий параметр для запуска Split");
            }
        }

        private eSplitParam GetSplitParam()
        {
            eSplitParam answer = eSplitParam.None;
            if (IsCheckedParamNewWindow) answer |= eSplitParam.OpenNewWindow;
            if (IsCheckedParamClipboard) answer |= eSplitParam.CopyToClipBoard;
            if (IsCheckedParamAddHeader) answer |= eSplitParam.GenerateHeader;
            if (IsCheckedParamUpdateRep) answer |= eSplitParam.DirectlyUpdateRep;
            return answer;
        }
    }
}
