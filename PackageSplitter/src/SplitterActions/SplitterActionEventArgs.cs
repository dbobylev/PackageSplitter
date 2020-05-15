using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.SplitterActions
{
    class SplitterActionEventArgs
    {
        public eSplitterActionType SplitterAction { get; private set; }
        public eSplitterObjectType SplitterObject { get; private set; }

        public SplitterActionEventArgs(eSplitterActionType splitterAction, eSplitterObjectType splitterObject)
        {
            SplitterAction = splitterAction;
            SplitterObject = splitterObject;
        }
    }
}
