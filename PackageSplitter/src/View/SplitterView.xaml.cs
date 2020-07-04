using DataBaseRepository.Model;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
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
using System.Text.RegularExpressions;

namespace PackageSplitter.View
{
    /// <summary>
    /// Interaction logic for ucSplitter.xaml
    /// </summary>
    public partial class SplitterView : UserControl
    {
        public SplitterView()
        {
            DataContext = new SplitterViewModel(SplitManager.Instance());
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var item = e.Item as SplitterElementViewModel;
            var elementType = item.ElementType;
            var elementName = item.Name;

            var answer =
                ((bool)RadioLinksALL.IsChecked || item.IsRequiried)
                && Regex.IsMatch(elementName, tbElementPattern.Text, RegexOptions.IgnoreCase) 
                && (  ((elementType == ePackageElementType.Function || elementType == ePackageElementType.Procedure) && (bool)chkbShowMethods.IsChecked) 
                   || (elementType == ePackageElementType.Variable && (bool)chkbShowVariables.IsChecked) 
                   || (elementType == ePackageElementType.Type && (bool)chkbShowTypes.IsChecked) 
                   || (elementType == ePackageElementType.Cursor && (bool)chkbShowCursors.IsChecked)
                   );

            e.Accepted = answer;
        }

        private void UpdateCollectionViewSource(object sender, RoutedEventArgs e)
        {
            ((CollectionViewSource)this.Resources["ViewSourceItems"]).View.Refresh();
        }

        private void chkBoxRepositoryUpdate_Checked(object sender, RoutedEventArgs e)
        {
            var box = sender as CheckBox;
            if ((bool)box.IsChecked)
            {
                box.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                box.FontWeight = FontWeights.Bold;
                MessageBox.Show("Внимание, при разбиение пакета, будет обновлён репозиторий (без комита)! Обязательно перепроверьте все вносимые изменения перед фиксацией.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                box.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                box.FontWeight = FontWeights.Normal;
            }
        }

        private void SplitNewBodyButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.Command.Execute(eSplitterObjectType.NewBody);
            UpdateCollectionViewSource(sender, e);
        }
    }
}
