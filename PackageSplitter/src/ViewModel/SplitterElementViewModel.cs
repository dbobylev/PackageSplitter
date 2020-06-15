using OracleParser.Model;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.ViewModel.Convertrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

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
        public eElementStateType OldSpecState { get => _model.OldSpec; set { _model.OldSpec = value; OnPropertyChanged(); ValidateState(); } }
        public eElementStateType OldBodyState { get => _model.OldBody; set { _model.OldBody = value; OnPropertyChanged(); ValidateState(); } }
        public eElementStateType NewSpecState { get => _model.NewSpec; set { _model.NewSpec = value; OnPropertyChanged(); ValidateState(); } }
        public eElementStateType NewBodyState { get => _model.NewBody; set { _model.NewBody = value; OnPropertyChanged(); ValidateState(); } }

        public bool IsRequiried { get => _model.IsRequiried; }
        public bool MakePrefix { 
            get => _model.MakePrefix; 
            set  
            {
                _model.MakePrefix = value;
                OnPropertyChanged();
            } 
        }

        public void UpdateIsRequiried()
        {
            OnPropertyChanged("IsRequiried");
            OnPropertyChanged("MakePrefix");
        }

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

        #region Validate
        private string _ErrorMessage;
        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            private set
            {
                _ErrorMessage = value;
                ShowError = string.IsNullOrEmpty(_ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged("ErrorMessage");
                OnPropertyChanged("ShowError");
            }
        }
        public Visibility ShowError { get; private set; } = Visibility.Collapsed;
        private void ValidateState()
        {
            var errorMsg = string.Empty;

            if (ElementType == ePackageElementType.Method)
            {
                if (NewSpecState == eElementStateType.Add && NewBodyState == eElementStateType.Empty)
                    errorMsg = "Необходимо добавить тело метода";
                else if ((OldSpecState == eElementStateType.Exist | OldSpecState == eElementStateType.Add) && OldBodyState == eElementStateType.Delete)
                    errorMsg = "Необходимо добавить тело метода";
            }
            else
            {
                if ((OldSpecState == eElementStateType.Exist | OldSpecState == eElementStateType.Add) && (OldBodyState == eElementStateType.Add | OldBodyState == eElementStateType.Exist))
                    errorMsg = "Нельзя объявить дважды";
                if (NewSpecState == eElementStateType.Add && NewBodyState == eElementStateType.Add)
                    errorMsg = "Нельзя объявить дважды";
            }

            ErrorMessage = errorMsg;
        }
        #endregion
    }
}
