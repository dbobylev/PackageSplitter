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
using PackageSplitter.Convertrs;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastOwnerUed();
        }

        private void SetLastOwnerUed()
        {
            Seri.Log.Verbose("SetLastUsedOwner begin");

            string LastOwnerUsed = Config.Instanse().LastOwnerUsed;
            Seri.Log.Verbose($"Load from settings: {LastOwnerUsed}");

            var value = cbOwners.ItemsSource.OfType<string>().Where(x => x == LastOwnerUsed).FirstOrDefault();
            if (value != null)
            {
                Seri.Log.Verbose($"SetLastUsedOwner find value: [{value}]");
                cbOwners.SelectedValue = value;
            }
            else
                Seri.Log.Verbose($"SetLastUsedOwner end");
        }

        private void btnLoadObject_Click(object sender, RoutedEventArgs e)
        {
            var obj = cbRepositoryObjects.SelectedItem as RepositoryObject;
            if (obj is null)
                return;
            var pac = OraParser.Instance().GetPackage(new RepositoryPackage(obj));
            var x = new Package(pac);
        }
    }
}
