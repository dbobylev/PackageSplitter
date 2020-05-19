using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model.SplitterGrid
{
    public class SplitterAction
    {
        public eCellSplitterActionType CellSplitterAction { get; private set; }
        public eSplitterObjectType SplitterObject { get; private set; }

        public SplitterAction(eCellSplitterActionType splitterAction, eSplitterObjectType splitterObject)
        {
            CellSplitterAction = splitterAction;
            SplitterObject = splitterObject;
        }
    }
}
