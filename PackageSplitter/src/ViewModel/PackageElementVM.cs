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
                NewSpecState = GetNewState(NewSpecState, cellAction.SplitterAction);

            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.NewBody))
                NewBodyState = GetNewState(NewBodyState, cellAction.SplitterAction);
        }

        private eElementStateType GetNewState(eElementStateType oldState, eSplitterCellActionType action)
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
                    if (oldState == eElementStateType.Exist)
                        answer = eElementStateType.Delete;
                    else if (oldState == eElementStateType.Add)
                        answer = eElementStateType.Exist;
                    else if (oldState == eElementStateType.CreateLink)
                        answer = eElementStateType.Exist;
                    break;
                case eSplitterCellActionType.MakeLink:
                    if (oldState == eElementStateType.Exist)
                        answer = eElementStateType.CreateLink;
                    break;
                default:
                    break;
            }
            return answer;
        }
    }
}
