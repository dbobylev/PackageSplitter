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
        public string TextToDisplay { get; set; } = string.Empty;

        public static readonly RoutedEvent SplitterCellActionEvent =
            EventManager.RegisterRoutedEvent("Demo",
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
            RaiseEvent(new SplitterCellActionEventArgs(SplitterCell.SplitterCellActionEvent, sender, eSplitterCellActionType.Add, eSplitterObjectType.NewBody));
        }
    }
}
