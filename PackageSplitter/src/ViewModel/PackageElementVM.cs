using PackageSplitter.Model;
using PackageSplitter.Splitter;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.ViewModel
{
    public class PackageElementVM : PropertyChangedBase
    {
        private PackageElement _model;

        public string Name { get => _model.PackageElementName; }
        public int ID { get; private set; }

        public eElementStateType OldSpecState { get => _model.OldSpec; set { _model.OldSpec = value; OnPropertyChanged(); } }
        public eElementStateType OldBodyState { get => _model.OldBody; set { _model.OldBody = value; OnPropertyChanged(); } }
        public eElementStateType NewSpecState { get => _model.NewSpec; set { _model.NewSpec = value; OnPropertyChanged(); } }
        public eElementStateType NewBodyState { get => _model.NewBody; set { _model.NewBody = value; OnPropertyChanged(); } }

        public PackageElementVM(PackageElement model, int index)
        {
            _model = model;
            ID = index;
        }

        public void PerformElementAction(SplitterCellAction cellAction)
        {
            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.OldSpec))
                OldSpecState = GetNewState(OldSpecState, cellAction.SplitterAction);

            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.OldBody))
                OldBodyState = GetNewState(OldBodyState, cellAction.SplitterAction);

            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.NewSpec))
                NewSpecState = GetNewState(NewSpecState, cellAction.SplitterAction, true);

            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.NewBody))
                NewBodyState = GetNewState(NewBodyState, cellAction.SplitterAction, true);
        }

        private eElementStateType GetNewState(eElementStateType oldState, eSplitterCellActionType action, bool IsNew = false)
        {
            eElementStateType answer = oldState;
            switch (action)
            {
                case eSplitterCellActionType.Add:
                    if (oldState == eElementStateType.Empty)
                        answer = eElementStateType.Add;
                    else if (oldState == eElementStateType.Delete)
                        answer = eElementStateType.Exist;
                    break;
                case eSplitterCellActionType.Delete:
                    if (oldState == eElementStateType.Exist || oldState == eElementStateType.CreateLink)
                        answer = eElementStateType.Delete;
                    else if (oldState == eElementStateType.Add)
                        answer = IsNew ? eElementStateType.Empty : eElementStateType.Exist;
                    break;
                case eSplitterCellActionType.MakeLink:
                    if (oldState == eElementStateType.Exist)
                        answer = eElementStateType.CreateLink;
                    else if (oldState == eElementStateType.Empty && IsNew)
                        answer = eElementStateType.Add;
                    break;
                default:
                    break;
            }
            return answer;
        }
    }
}
