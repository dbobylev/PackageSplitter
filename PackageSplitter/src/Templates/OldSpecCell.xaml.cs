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

namespace PackageSplitter.Templates
{
    /// <summary>
    /// Interaction logic for OldSpecCell.xaml
    /// </summary>
    public partial class OldSpecCell : UserControl
    {
        public event Action DeleteOldSpec;

        public OldSpecCell()
        {
            InitializeComponent();
        }
    }
}
