using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PackageSplitter.Splitter
{
    public delegate void SplitterCellActionEventHandler(object sender, SplitterCellActionEventArgs args);

    public class SplitterCellActionEventArgs : RoutedEventArgs
    {
        public string ElementName { get; private set; }
        public eSplitterCellActionType SplitterAction { get; private set; }
        public eSplitterObjectType SplitterObject { get; private set; }

        public SplitterCellActionEventArgs(
            RoutedEvent routedEvent, 
            object source, 
            string elementName,
            eSplitterCellActionType splitterAction, 
            eSplitterObjectType splitterObject) 
            : base(routedEvent, source)
        {
            ElementName = elementName;
            SplitterAction = splitterAction;
            SplitterObject = splitterObject;
        }
    }

}
