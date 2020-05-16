using PackageSplitter.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.ViewModel
{
    public class PackageElementVM : PropertyChangedBase
    {
        private PackageElement _model;

        public string Name { get => _model.Name; }
        public int ID { get; private set; }

        public eElementStateType OldSpec { get => _model.OldSpec; }
        public eElementStateType OldBody { get => _model.OldBody; set { _model.OldBody = value; OnPropertyChanged(); } }
        public eElementStateType NewSpec { get => _model.NewSpec; }
        public eElementStateType NewBody { get => _model.NewBody; }

        public PackageElementVM(PackageElement model, int index)
        {
            _model = model;
            ID = index;
        }
    }
}
