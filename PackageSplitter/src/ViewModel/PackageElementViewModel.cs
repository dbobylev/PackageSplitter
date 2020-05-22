using OracleParser.Model;
using PackageSplitter.Model;
using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.ViewModel
{
    public class PackageElementViewModel : PropertyChangedBase
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public ePackageElementType PackageElementType { get; private set; }

        private eElementStateType _OldSpecState;
        public eElementStateType OldSpecState { get => _OldSpecState; set { _OldSpecState = value; OnPropertyChanged(); } }

        private eElementStateType _OldBodyState;
        public eElementStateType OldBodyState { get => _OldBodyState; set { _OldBodyState = value; OnPropertyChanged(); } }
        
        private eElementStateType _NewSpecState;
        public eElementStateType NewSpecState { get => _NewSpecState; set { _NewSpecState = value; OnPropertyChanged(); } }

        private eElementStateType _NewBodyState;
        public eElementStateType NewBodyState { get => _NewBodyState; set { _NewBodyState = value; OnPropertyChanged(); } }

        public PackageElementViewModel(int index)
        {
            ID = index;
        }

        /*public void PerformElementAction(CellSplitterAction cellAction)
        {
            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.OldSpec))
                OldSpecState = GetNewState(OldSpecState, cellAction.CellSplitterAction);

            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.OldBody))
                OldBodyState = GetNewState(OldBodyState, cellAction.CellSplitterAction);

            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.NewSpec))
                NewSpecState = GetNewState(NewSpecState, cellAction.CellSplitterAction, true);

            if (cellAction.SplitterObject.HasFlag(eSplitterObjectType.NewBody))
                NewBodyState = GetNewState(NewBodyState, cellAction.CellSplitterAction, true);
        }

        private eElementStateType GetNewState(eElementStateType oldState, eCellSplitterActionType action, bool IsNew = false)
        {
            eElementStateType answer = oldState;
            switch (action)
            {
                case eCellSplitterActionType.Add:
                    if (oldState == eElementStateType.Empty)
                        answer = eElementStateType.Add;
                    else if (oldState == eElementStateType.Delete)
                        answer = eElementStateType.Exist;
                    break;
                case eCellSplitterActionType.Delete:
                    if (oldState == eElementStateType.Exist || oldState == eElementStateType.CreateLink)
                        answer = eElementStateType.Delete;
                    else if (oldState == eElementStateType.Add)
                        answer = IsNew ? eElementStateType.Empty : eElementStateType.Exist;
                    break;
                case eCellSplitterActionType.MakeLink:
                    if (oldState == eElementStateType.Exist)
                        answer = eElementStateType.CreateLink;
                    else if (oldState == eElementStateType.Empty && IsNew)
                        answer = eElementStateType.Add;
                    break;
                default:
                    break;
            }
            return answer;
        }*/
    }
}
