﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Splitter
{
    public class SplitterCellAction :CellAction
    {
        public int ID { get; private set; }

        public SplitterCellAction(int index, eSplitterObjectType splitterObject, eSplitterCellActionType splitterAction)
            :base(splitterAction, splitterObject)
        {
            ID = index;
        }

        public SplitterCellAction(int indext, CellAction cellAction):base(cellAction.SplitterAction, cellAction.SplitterObject)
        {
            ID = indext;
        }
    }
}
