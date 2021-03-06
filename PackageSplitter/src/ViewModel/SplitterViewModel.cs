﻿using DataBaseRepository.Model;
using OracleParser.Model;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Documents;
using System.Windows.Media;
using PackageSplitter.ViewModel.Convertrs;

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

        public bool IsCheckedParamNewWindow { get; set; } = true;
        public bool IsCheckedParamClipboard { get; set; } = true;
        public bool IsCheckedParamAddHeader { get; set; } = true;
        public bool IsCheckedParamUpdateRep { get; set; } = false;

        private bool RepositoryObjectWasUpdated = false;

        public ObservableCollection<SplitterElementViewModel> ElementsViewModel { get; private set; }

        public SolidColorBrush LinksRadioBackground
        {
            get
            {
                if (ElementsViewModel == null || ElementsViewModel.Count == 0 || _splitter.Elements.All(x => !x.IsRequiried || x.MakePrefix))
                    return "cRadioLinksDefault".FindResource<SolidColorBrush>();
                else
                    return "cCellYellow".FindResource<SolidColorBrush>();
            }
        }

        public SplitterViewModel(ISplitManager splitManager)
        {
            _SplitManager = splitManager;
            _SplitManager.OraclePackageSetted += SetModel;
            ElementsViewModel = new ObservableCollection<SplitterElementViewModel>();
            
            SplitCommand = new RelayCommand(RunSplit, (x) => _splitter != null && !RepositoryObjectWasUpdated);
            SaveSplitterCommand = new RelayCommand(SaveModel, (x) => _SplitterSaver != null);
            LoadSplitterCommand = new RelayCommand(LoadModel, (x) => _SplitterSaver != null);
        }

        public void SetModel(Package parsedPackage)
        {
            RepositoryObjectWasUpdated = false;
            _SplitterSaver = new SplitterSaver(parsedPackage.repositoryPackage.ObjectName, parsedPackage.SHA);
            _splitter = new Splitter(parsedPackage);
            _SplitManager.SetSplitterPackage(_splitter);

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
                try
                {
                    if (splitterObjectType == eSplitterObjectType.NewBody && AnalyzeLinks())
                        return;

                    if (IsCheckedParamUpdateRep && !splitterObjectType.IsNew())
                    {
                        var res = MessageBox.Show("Внимание! Сейчас в репозитории будут обновленны исходный файлы пакета: тело и спецификация. Продолжить?", "Обновление репозитория", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (res == MessageBoxResult.Yes)
                            RepositoryObjectWasUpdated = true;
                        else if (res == MessageBoxResult.No)
                            return;
                    }

                    _SplitManager.RunSplit(splitterObjectType, GetSplitParam());
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                _SplitManager.SetSplitterPackage(_splitter);
                FillElements();
            }
            OnPropertyChanged("HasRequiried");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Да - есть неразрешенные проблемы, прекращаем выполнение. Нет - продолжаем генерацию текста</returns>
        private bool AnalyzeLinks()
        {
            var answer = _SplitManager.AnalizeLinks();

            if (answer)
            {
                MessageBoxResult result = MessageBox.Show("При анализе нового тела пакета найдены неразрешенные ссылки. Продолжить?", "Links", MessageBoxButton.YesNo, MessageBoxImage.Information);
                answer = result == MessageBoxResult.No;
            }

            foreach (var item in ElementsViewModel)
                item.UpdateIsRequiried();
            OnPropertyChanged("LinksRadioBackground");

            var WrongPrefix = _splitter.Elements.Where(x => x.MakePrefix && (x.OldSpec == eElementStateType.Delete || x.OldSpec == eElementStateType.Empty)).ToList();
            if (WrongPrefix.Any())
            {
                MessageBox.Show($"Указан префикс на отсутствующию спецификацию в исходном пакете: ({string.Join(", ", WrongPrefix.Select(x => x.PackageElementName))})", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                answer = true;
            }

            return answer;
        }
    }
}
