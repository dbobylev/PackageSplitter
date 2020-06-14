﻿using DataBaseRepository.Model;
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
        private Splitter _splitter;
        private SplitterSaver _SplitterSaver;
        public RelayCommand SplitCommand { get; private set; }
        public RelayCommand SaveSplitterCommand { get; private set; }
        public RelayCommand LoadSplitterCommand { get; private set; }
        public RelayCommand RunAnalyzeLinksCommand { get; private set; }

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
            
            SplitCommand = new RelayCommand(RunSplit, (x) => _splitter != null);
            SaveSplitterCommand = new RelayCommand(SaveModel, (x) => _SplitterSaver != null);
            LoadSplitterCommand = new RelayCommand(LoadModel, (x) => _SplitterSaver != null);
            RunAnalyzeLinksCommand = new RelayCommand(AnalyzeLinks, (x) => _splitter != null);
        }

        public void SetModel(Package parsedPackage)
        {
            _SplitterSaver = new SplitterSaver(parsedPackage.repositoryPackage.ObjectName, parsedPackage.SHA);
            _splitter = new Splitter(parsedPackage);
            FillElements();
        }

        private void FillElements()
        {
            ElementsViewModel.Clear();
            for (int i = 0; i < _splitter.Elements.Count; i++)
                ElementsViewModel.Add(new SplitterElementViewModel(_splitter.Elements[i]));
        }

        private void RunSplit(object param)
        {
            if (param is eSplitterObjectType splitterObjectType)
            {
                _SplitManager.LoadSplitterPackage(_splitter);
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

        public void SaveModel(object obj)
        {
            _SplitterSaver.Save(_splitter);
        }

        public void LoadModel(object obj)
        {
            if (_SplitterSaver.Load(out Splitter splitter))
            {
                _splitter = splitter;
                FillElements();
            }
        }

        public void AnalyzeLinks(object obj)
        {
            _SplitManager.LoadSplitterPackage(_splitter);
            _SplitManager.AnalizeLinks();

            foreach (var item in ElementsViewModel)
                item.UpdateIsRequiried();
        }
    }
}
