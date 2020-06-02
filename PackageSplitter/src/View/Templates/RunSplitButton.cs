using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PackageSplitter.View.Templates
{
    class RunSplitButton : Button
    {
        public eSplitterObjectType SplitterObjectType { get; set; }

        public RunSplitButton() : base()
        {
            Margin = new Thickness(5);
            Padding = new Thickness(10, 0, 10, 0);
        }
    }
}
