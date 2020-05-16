using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Splitter
{
    public class SplitterCellAction
    {
        public int ID { get; private set; }
        public eSplitterCellActionType SplitterAction { get; private set; }
        public eSplitterObjectType SplitterObject { get; private set; }

        public SplitterCellAction(int index, eSplitterObjectType splitterObject, eSplitterCellActionType splitterAction)
        {
            ID = index;
            SplitterAction = splitterAction;
            SplitterObject = splitterObject;
        }
    }
}
