using PackageSplitter.Model;
using PackageSplitter.Splitter;
using PackageSplitter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PackageSplitter.View
{
    /// <summary>
    /// Interaction logic for ucSplitter.xaml
    /// </summary>
    public partial class SplitterView : UserControl
    {
        PackageVM packageVM;

        public SplitterView()
        {
            InitializeComponent();

            packageVM = MainGrid.DataContext as PackageVM;
        }

        public void AddElements(IEnumerable<PackageElement> elements)
        {
            packageVM.AddElements(elements.Select((x, i) => new PackageElementVM(x, i)));
        }

        private void SplitterCell_SplitterCellAction(object sender, SplitterCellActionEventArgs args)
        {
            packageVM.PerformElementAction(args.CellAction);
        }
    }
}
