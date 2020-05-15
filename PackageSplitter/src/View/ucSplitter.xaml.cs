using PackageSplitter.Model;
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
    public partial class ucSplitter : UserControl
    {
        PackageVM packageVM;

        public ucSplitter()
        {
            InitializeComponent();

            packageVM = MainGrid.DataContext as PackageVM;
        }

        public void AddElements(IEnumerable<PackageElement> elements)
        {
            packageVM.AddElements(elements.Select(x => new PackageElementVM(x)));
        }

        private void OldSpecCell_DeleteOldSpec()
        {

        }
    }
}
