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

        public eElementStateType OldSpec { get => _model.OldSpec; }
        public eElementStateType OldBody { get => _model.OldBody; set { _model.OldBody = value; } }
        public eElementStateType NewSpec { get => _model.NewSpec; }
        public eElementStateType NewBody { get => _model.NewBody; }

        public PackageElementVM(PackageElement model)
        {
            _model = model;
        }

        public event Action DeleteOldSpecVM;
    }
}
