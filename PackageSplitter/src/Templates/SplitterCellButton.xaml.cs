using PackageSplitter.Splitter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PackageSplitter.Templates
{
    /// <summary>
    /// Interaction logic for SplitterCellButton.xaml
    /// </summary>
    public partial class SplitterCellButton : UserControl
    {
        SplitterCellActionHandler _splitterCellActionHandler;
        private SplitterCellAction _CellAction;

        public SplitterCellButton(SplitterCellAction cellAction, SplitterCellActionHandler ClickHandler)
        {
            InitializeComponent();

            _CellAction = cellAction;
            _splitterCellActionHandler = ClickHandler;

            switch (cellAction.SplitterAction)
            {
                case eSplitterCellActionType.Add:
                    mainButton.Content = "+";
                    break;
                case eSplitterCellActionType.Delete:
                    mainButton.Content = "-";
                    break;
                case eSplitterCellActionType.MakeLink:
                    mainButton.Content = "L";
                    break;
                default:
                    break;
            }
        }

        private void mainButton_Click(object sender, RoutedEventArgs e)
        {
            _splitterCellActionHandler(this, _CellAction);
        }
    }
}
