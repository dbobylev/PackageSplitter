using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model.SplitterGrid
{
    public class CellSplitterAction :SplitterAction
    {
        public int ID { get; private set; }

        public CellSplitterAction(int index, eSplitterObjectType splitterObject, eCellSplitterActionType splitterAction)
            :base(splitterAction, splitterObject)
        {
            ID = index;
        }

        public CellSplitterAction(int indext, SplitterAction cellAction):base(cellAction.CellSplitterAction, cellAction.SplitterObject)
        {
            ID = indext;
        }
    }
}
