using DataBaseRepository.Model;
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
using System.Windows.Shapes;

namespace PackageSplitter.View
{
    /// <summary>
    /// Interaction logic for ParserWindow.xaml
    /// </summary>
    public partial class ParserWindow : Window
    {
        private RepositoryPackage _repositoryPackage;

        public ParserWindow(RepositoryPackage repositoryPackage)
        {
            _repositoryPackage = repositoryPackage;
            InitializeComponent();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ParserViewModel(_repositoryPackage, (x) => Close());
        }
    }
}
