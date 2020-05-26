using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PackageSplitter.View.Templates
{
    class SplitterCellButton : Button
    {
        public Dictionary<eSplitterObjectType, eElementStateType> NewStates { get; private set; }
        public eSplitterCellButtonType SplitterCellButtonType { get; private set; }

        public SplitterCellButton( eSplitterCellButtonType buttonType, Dictionary<eSplitterObjectType, eElementStateType> newStates)
        {
            SplitterCellButtonType = buttonType;
            NewStates = newStates;
            switch (buttonType)
            {
                case eSplitterCellButtonType.AddButton: Content = "+"; break;
                case eSplitterCellButtonType.RemoveButton: Content = "-"; break;
                case eSplitterCellButtonType.LinkButton: Content = "L"; break;
                case eSplitterCellButtonType.CopyButton: Content = "C"; break;
                case eSplitterCellButtonType.MoveButton: Content = "M"; break;
                default:
                    break;
            }

            Margin = new Thickness(3);
            Padding = new Thickness(3);
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Width = 24;
            Height = 24;
        }
    }
}
