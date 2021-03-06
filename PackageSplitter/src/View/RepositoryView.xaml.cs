﻿using DataBaseRepository.Model;
using OracleParser;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.src.View;
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

namespace PackageSplitter.View
{
    /// <summary>
    /// Interaction logic for RepositoryObjectView.xaml
    /// </summary>
    public partial class RepositoryView : UserControl
    {
        public RepositoryView()
        {
            DataContext = new RepositoryViewModel();
            InitializeComponent();
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

        public void SetLastFileUsed()
        {
            if (cbRepositoryObjects.Items.Count > 0)
            {
                var items = cbRepositoryObjects.Items.Cast<RepositoryObject>();
                var LastSelected = items
                    .Select((x, i) => new { val = x, index = i })
                    .FirstOrDefault(x => x.val.RepFilePath == Config.Instanse().LastFileUsed);
                if (LastSelected != null)
                    cbRepositoryObjects.SelectedIndex = LastSelected.index;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastOwnerUed();
            SetLastFileUsed();
        }
    }
}
