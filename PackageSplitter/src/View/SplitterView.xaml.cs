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
        private SplitterPackageViewModel _PackageViewModel;

        private eSplitParam _defaultNewObjParam = eSplitParam.CopyToClipBoard | eSplitParam.GenerateHeader | eSplitParam.OpenNewWindow;

        public SplitterView()
        {
            InitializeComponent();

            _PackageViewModel = MainGrid.DataContext as SplitterPackageViewModel;
        }

        public void SetModel(SplitterPackage splitterPackage)
        {
            _PackageViewModel.SetModel(splitterPackage);
        }

        private void btnNewSpec_Click(object sender, RoutedEventArgs e)
        {
            SplitManager.Instance().SetSplitterPackage(_PackageViewModel.GetSplitterPackage());
            SplitManager.Instance().RunSplitNewSpec(eSplitterObjectType.NewSpec, _defaultNewObjParam);
        }

        private void btnNewBody_Click(object sender, RoutedEventArgs e)
        {
            SplitManager.Instance().SetSplitterPackage(_PackageViewModel.GetSplitterPackage());
            SplitManager.Instance().RunSplitNewBody(eSplitterObjectType.NewBody, _defaultNewObjParam);
        }

        private void btnOldSpec_Click(object sender, RoutedEventArgs e)
        {
            SplitManager.Instance().SetSplitterPackage(_PackageViewModel.GetSplitterPackage());
            SplitManager.Instance().RunSplitOldSpec(_defaultNewObjParam);
        }
    }
}
