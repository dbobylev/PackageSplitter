using PackageSplitter.Model;
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
    /// Interaction logic for OldSpecCell.xaml
    /// </summary>
    public partial class SplitterCell : UserControl
    {
        public static DependencyProperty ElementStateProperty = DependencyProperty.Register("ElementState", typeof(eElementStateType), typeof(SplitterCell));
        public eElementStateType ElementState { get; set; }

        //public eSplitterObjectType CellType { get; set; }

        /*
        public static readonly RoutedEvent SplitterCellActionEvent =
            EventManager.RegisterRoutedEvent("SplitterCellActionEventRoute",
            RoutingStrategy.Bubble,
            typeof(CellSplitterActionEventHandler),
            typeof(SplitterCell));
        */

        /*
        public event CellSplitterActionEventHandler SplitterCellAction
        {
            add { AddHandler(SplitterCellActionEvent, value); }
            remove { RemoveHandler(SplitterCellActionEvent, value); }
        }*/

        public event Action<eElementStateType> Go;

        public SplitterCell()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Go(eElementStateType.Add);
        }

        /*
        private void AnyButton_Click(object sender, CellSplitterAction e)
        {
            RaiseEvent(new CellSplitterActionEventArgs(
                routedEvent: SplitterCell.SplitterCellActionEvent,
                source: sender,
                cellAction: e));
        }
        */
        /*private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            var buttons = _cellButtonsFactory.GetButtons((int)GetValue(PackageElementIDProperty), CellType, AnyButton_Click);

            foreach (var item in buttons)
                mainStack.Children.Add(item);
        }*/
    }
}
