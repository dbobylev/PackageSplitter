using PackageSplitter.Model;
using PackageSplitter.Model.SplitterGrid;
using PackageSplitter.ViewModel;
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

        public SplitterCell()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var z = this.DataContext as SplitterPackageElementViewModel;
            z.DoAction(eElementStateType.Add);
        }
    }
}
