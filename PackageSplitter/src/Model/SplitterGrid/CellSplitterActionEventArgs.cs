using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PackageSplitter.Model.SplitterGrid
{
    public delegate void CellSplitterActionHandler(object sender, CellSplitterAction cellAction);
    public delegate void CellSplitterActionEventHandler(object sender, CellSplitterActionEventArgs args);

    public class CellSplitterActionEventArgs : RoutedEventArgs
    {
        public CellSplitterAction CellAction { get; private set; }

        public CellSplitterActionEventArgs(CellSplitterAction cellAction, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            CellAction = cellAction;
        }
    }
}
