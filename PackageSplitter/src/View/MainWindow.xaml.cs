using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataBaseRepository;
using DataBaseRepository.Model;
using OracleParser;
using PackageSplitter.ViewModel.Convertrs;
using PackageSplitter.Model;

namespace PackageSplitter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RepositoryObjectView_PushPackageElements(SplitterPackage obj)
        {
            Splitter.SetModel(obj);
        }
    }
}
