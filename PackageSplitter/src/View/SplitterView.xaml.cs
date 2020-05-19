using DataBaseRepository.Model;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.Model.SplitterGrid;
using PackageSplitter.src.View;
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
        PackageViewModel _PackageViewModel;
        SplitManager _SplitManager;

        public SplitterView()
        {
            InitializeComponent();

            _PackageViewModel = MainGrid.DataContext as PackageViewModel;
        }

        public void AddElements(IEnumerable<PackageElement> elements, RepositoryPackage repositoryObject)
        {
            _PackageViewModel.AddElements(elements.Select((x, i) => new PackageElementViewModel(x, i)));
            _SplitManager = new SplitManager(repositoryObject);
        }

        private void SplitterCell_SplitterCellAction(object sender, CellSplitterActionEventArgs args)
        {
            _PackageViewModel.PerformElementAction(args.CellAction);
        }

        private void btnTextNewSpec_Click(object sender, RoutedEventArgs e)
        {
            string text = _SplitManager.GetNewSpecText(_PackageViewModel.GetNewSpec);
            TextWindow tw = new TextWindow(text);
            tw.Show();
        }
    }
}
