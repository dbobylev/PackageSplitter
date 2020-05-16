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
    /// Interaction logic for OldSpecCell.xaml
    /// </summary>
    public partial class SplitterCell : UserControl
    {
        public static DependencyProperty TextToDisplayProperty = DependencyProperty.Register("TextToDisplay", typeof(string), typeof(SplitterCell));
        public string TextToDisplay { get; set; }

        public static DependencyProperty PackageElementIDProperty = DependencyProperty.Register("PackageElementID", typeof(int), typeof(SplitterCell));
        public int PackageElementID { get; set; }

        public static readonly RoutedEvent SplitterCellActionEvent =
            EventManager.RegisterRoutedEvent("SplitterCellActionEventRoute",
            RoutingStrategy.Bubble,
            typeof(SplitterCellActionEventHandler),
            typeof(SplitterCell));

        public event SplitterCellActionEventHandler SplitterCellAction
        {
            add { AddHandler(SplitterCellActionEvent, value); }
            remove { RemoveHandler(SplitterCellActionEvent, value); }
        }

        public SplitterCell()
        {
            Seri.Log.Verbose("SplitterCell InitializeComponent");
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Seri.Log.Verbose("Button_Click " + GetValue(PackageElementIDProperty));
            RaiseEvent(new SplitterCellActionEventArgs(
                routedEvent: SplitterCell.SplitterCellActionEvent,
                source: sender,
                elementID: (int)GetValue(PackageElementIDProperty),
                splitterAction: eSplitterCellActionType.Add, 
                splitterObject: eSplitterObjectType.NewBody));;
        }
    }
}
