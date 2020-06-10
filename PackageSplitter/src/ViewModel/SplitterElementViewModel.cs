using OracleParser.Model;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.ViewModel.Convertrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageSplitter.ViewModel
{
    public class SplitterElementViewModel : PropertyChangedBase
    {
        private SplitterElement _model;

        public SplitterElementViewModel(SplitterElement model)
        {
            _model = model;
        }

        public string Name => _model.PackageElementName;
        public ePackageElementType ElementType => _model.PackageElementType;
        public string ElementTypeStr => ElementType.GetDescription();
        public eElementStateType OldSpecState { get => _model.OldSpec; set { _model.OldSpec = value; OnPropertyChanged(); } }
        public eElementStateType OldBodyState { get => _model.OldBody; set { _model.OldBody = value; OnPropertyChanged(); } }
        public eElementStateType NewSpecState { get => _model.NewSpec; set { _model.NewSpec = value; OnPropertyChanged(); } }
        public eElementStateType NewBodyState { get => _model.NewBody; set { _model.NewBody = value; OnPropertyChanged(); } }

        public void UpdateStates(Dictionary<eSplitterObjectType, eElementStateType> buttonNewStates)
        {
            var newStates = new Dictionary<eSplitterObjectType, eElementStateType>(buttonNewStates);
            Seri.Log.Debug($"UpdateStates: {Name} {string.Join(",", newStates.Select(x => $"[{x.Key}: {x.Value}]"))}");

            // Если удаляем в OldBody ссылку, OldSpec при этом не трогаем
            if (OldBodyState == eElementStateType.CreateLink
                && newStates.ContainsKey(eSplitterObjectType.OldBody)
                && newStates[eSplitterObjectType.OldBody] == eElementStateType.Delete
                && newStates.ContainsKey(eSplitterObjectType.OldSpec))
                newStates.Remove(eSplitterObjectType.OldSpec);

            // Если в OldBody Ссылка и удаляется NewSpec, удаляем так же ссылку
            if (OldBodyState == eElementStateType.CreateLink
                && newStates.ContainsKey(eSplitterObjectType.NewSpec)
                && newStates[eSplitterObjectType.NewSpec] == eElementStateType.Delete)
                newStates.Add(eSplitterObjectType.OldBody, eElementStateType.Delete);

            foreach(var key in newStates.Keys)
                switch (key)
                {
                    case eSplitterObjectType.OldSpec: OldSpecState = newStates[key]; break;
                    case eSplitterObjectType.OldBody: OldBodyState = newStates[key]; break;
                    case eSplitterObjectType.NewSpec: NewSpecState = newStates[key]; break;
                    case eSplitterObjectType.NewBody: NewBodyState = newStates[key]; break;
                    default: break;
                }
        }
    }
}
