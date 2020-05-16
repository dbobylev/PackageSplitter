using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PackageSplitter.ViewModel
{
    public class PackageVM : PropertyChangedBase
    {
        private ObservableCollection<PackageElementVM> _elements;

        public ObservableCollection<PackageElementVM> Elements { get => _elements; set { _elements = value; } }

        public PackageVM()
        {
            _elements = new ObservableCollection<PackageElementVM>();
        }

        public void AddElements(IEnumerable<PackageElementVM> newElements)
        {
            foreach (var item in newElements)
            {
                _elements.Add(item);
            }
            //OnPropertyChanged("Elements");
        }
    }
}
