﻿using OracleParser.Model;
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

        public SplitterPackageViewModel()
        {
            ElementsViewModel = new ObservableCollection<SplitterPackageElementViewModel>();
        }

        public void SetModel(SplitterPackage model)
        {
            _model = model;

            for (int i = 0; i < _model.Elements.Count; i++)
                ElementsViewModel.Add(new SplitterPackageElementViewModel(_model.Elements[i]));
        }

        public SplitterPackage GetSplitterPackage()
        {
            return _model;
        }
    }
}
