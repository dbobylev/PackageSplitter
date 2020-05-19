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
    public class PackageViewModel : PropertyChangedBase
    {
        private ObservableCollection<PackageElementViewModel> _elements;

        public ObservableCollection<PackageElementViewModel> Elements { get => _elements; set { _elements = value; } }

        public PackageViewModel()
        {
            _elements = new ObservableCollection<PackageElementViewModel>();
        }

        public void AddElements(IEnumerable<PackageElementViewModel> newElements)
        {
            foreach (var item in newElements)
            {
                _elements.Add(item);
            }
        }

        public void PerformElementAction(CellSplitterAction cellAction)
        {
            _elements.Where(x => x.ID == cellAction.ID).FirstOrDefault().PerformElementAction(cellAction);
        }

        public PieceOfCode[] GetNewSpec => _elements.Select(x => x.Model).Where(x => x.NewSpec == eElementStateType.Add).Select(x => x.PosSpec).ToArray();
        public PieceOfCode[] GetNewBody => _elements.Select(x => x.Model).Where(x => x.NewBody == eElementStateType.Add).Select(x => x.PosBody).ToArray();
        public PieceOfCode[] GetOldSpec => _elements.Select(x => x.Model).Where(x => x.OldSpec == eElementStateType.Add || x.OldSpec == eElementStateType.Delete).Select(x => x.PosSpec).ToArray();
        public PieceOfCode[] GetOldBody => _elements.Select(x => x.Model).Where(x => x.OldBody == eElementStateType.Delete || x.OldBody == eElementStateType.CreateLink).Select(x => x.PosBody).ToArray();
    }
}
