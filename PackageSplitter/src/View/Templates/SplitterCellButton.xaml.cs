using PackageSplitter.Model.SplitterGrid;
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

namespace PackageSplitter.View.Templates
{
    /// <summary>
    /// Interaction logic for SplitterCellButton.xaml
    /// </summary>
    public partial class SplitterCellButton : UserControl
    {
        CellSplitterActionHandler _splitterCellActionHandler;
        private CellSplitterAction _CellAction;

        public SplitterCellButton(CellSplitterAction cellAction, CellSplitterActionHandler ClickHandler)
        {
            InitializeComponent();

            _CellAction = cellAction;
            _splitterCellActionHandler = ClickHandler;

            switch (cellAction.CellSplitterAction)
            {
                case eCellSplitterActionType.Add:
                    mainButton.Content = "V";
                    break;
                case eCellSplitterActionType.Delete:
                    mainButton.Content = "X";
                    break;
                case eCellSplitterActionType.MakeLink:
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
