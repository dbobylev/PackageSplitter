using DataBaseRepository.Model;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.Model.SplitterGrid;
using PackageSplitter.src.View;
using PackageSplitter.View.Templates;
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
using OracleParser.Model.PackageModel;

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

            _PackageViewModel = uc.DataContext as SplitterPackageViewModel;
        }

        public void SetModel(SplitterPackage splitterPackage)
        {
            _PackageViewModel.SetModel(splitterPackage);
        }

        private void RunSplitButton_Click(object sender, RoutedEventArgs e)
        {
            var SplitterObjectType = (sender as RunSplitButton).SplitterObjectType;

            SplitManager.Instance().SetSplitterPackage(_PackageViewModel.GetSplitterPackage());
            SplitManager.Instance().RunSplit(SplitterObjectType, _defaultNewObjParam);
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var elementType = (e.Item as SplitterPackageElementViewModel).ElementType;
            e.Accepted = (elementType == ePackageElementType.Method && (bool)chkbShowMethods.IsChecked) ||
                         (elementType == ePackageElementType.Variable && (bool)chkbShowVariables.IsChecked) ||
                         (elementType == ePackageElementType.Type && (bool)chkbShowTypes.IsChecked);
        }

        private void chkbShow_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ((CollectionViewSource)this.Resources["ViewSourceItems"]).View.Refresh();
        }
    }
}
