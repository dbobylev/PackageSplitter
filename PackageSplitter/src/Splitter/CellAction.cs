using PackageSplitter.Splitter;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Splitter
{
    public class CellAction
    {
        public eSplitterCellActionType SplitterAction { get; private set; }
        public eSplitterObjectType SplitterObject { get; private set; }

        public CellAction(eSplitterCellActionType splitterAction, eSplitterObjectType splitterObject)
        {
            SplitterAction = splitterAction;
            SplitterObject = splitterObject;
        }
    }
}
