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

namespace PackageSplitter.src.View
{
    /// <summary>
    /// Interaction logic for TextWindow.xaml
    /// </summary>
    public partial class TextWindow : Window
    {
        public TextWindow(string text)
        {
            InitializeComponent();
            tbMain.Text = text;
        }
    }
}
