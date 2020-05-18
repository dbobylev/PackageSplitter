using DataBaseRepository.Model;
using OracleParser;
using PackageSplitter.Model;
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
    /// Interaction logic for RepositoryObjectView.xaml
    /// </summary>
    public partial class RepositoryObjectView : UserControl
    {
        public RepositoryObjectView()
        {
            InitializeComponent();
        }

        public event Action<PackageElement[]> PushPackageElements;

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
            var repositoryObject = cbRepositoryObjects.SelectedItem as RepositoryObject;
            if (repositoryObject is null)
                return;

            var repositoryPackage = new RepositoryPackage(repositoryObject);
            var parsedPackage = OraParser.Instance().GetPackage(repositoryPackage);
            var PackageModel = new Package(parsedPackage);
            var ElementsList = PackageModel.Elements.ToArray();
            PushPackageElements?.Invoke(ElementsList);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastOwnerUed();
        }
    }
}
