using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PackageSplitter.Splitter
{
    public delegate void SplitterCellActionHandler(object sender, SplitterCellAction cellAction);
    public delegate void SplitterCellActionEventHandler(object sender, SplitterCellActionEventArgs args);

    public class SplitterCellActionEventArgs : RoutedEventArgs
    {
        public SplitterCellAction CellAction { get; private set; }

        public SplitterCellActionEventArgs(
            RoutedEvent routedEvent, 
            object source,
            SplitterCellAction cellAction
            ) 
            : base(routedEvent, source)
        {
            CellAction = cellAction;
        }
    }
}
