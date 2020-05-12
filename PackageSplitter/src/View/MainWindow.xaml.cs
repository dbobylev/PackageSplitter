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
using OracleParser;

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

            var descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.ItemsSourceProperty, typeof(ComboBox));

            descriptor.AddValueChanged(cbOwners, (sender, e) =>
            {
                Seri.Log.Verbose("In AddValueChanged");
                var LastOwnerUsed = Config.Instanse().LastOwnerUsed;
                ComboBox box = (sender as ComboBox);
                var value = box.ItemsSource.OfType<string>().Where(x => x == LastOwnerUsed).FirstOrDefault();
                if (value != null)
                    box.SelectedValue = value;
            });
        }
    }
}
