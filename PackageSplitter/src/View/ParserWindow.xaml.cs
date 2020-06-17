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
        public ParserWindow(RepositoryPackage repositoryPackage)
        {
            var VM = new ParserViewModel(repositoryPackage);
            VM.CloseAction = () => Close();
            DataContext = VM;

            InitializeComponent();
        }
    }
}
