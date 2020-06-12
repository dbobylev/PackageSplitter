﻿using DataBaseRepository.Model;
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
                Regex.IsMatch(elementName, tbElementPattern.Text, RegexOptions.IgnoreCase) 
                && (  (elementType == ePackageElementType.Method && (bool)chkbShowMethods.IsChecked) 
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
    }
}
