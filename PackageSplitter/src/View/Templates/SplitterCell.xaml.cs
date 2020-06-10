﻿using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PackageSplitter.View.Templates
{
    /// <summary>
    /// Interaction logic for OldSpecCell.xaml
    /// </summary>
    public partial class SplitterCell : UserControl
    {
        private bool ButtonsWasLoaded = false;

        private static SplitterCellButtonFactory ButtonFactory = new SplitterCellButtonFactory();

        public static DependencyProperty ElementStateTypeProperty = DependencyProperty.Register("ElementStateType", typeof(eElementStateType), typeof(SplitterCell));
        public eElementStateType ElementStateType { get; set; }

        public static DependencyProperty PackageElementTypeProperty = DependencyProperty.Register("PackageElementType", typeof(ePackageElementType), typeof(SplitterCell));
        public ePackageElementType PackageElementType { get; set; }

        public eSplitterObjectType SplitterObjectType { get; set; }

        public SplitterCell()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as SplitterElementViewModel;
            var button = sender as SplitterCellButton;
            viewModel.UpdateStates(button.NewStates);
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            /* Создание кнопок вынесено из конструктора, так как там еще собраны не все данные для построения кнопок
             * При прокрутке элементов, они пропадают с экрана и заново появляются, в этот момент повторно отрабатывает uc_Loaded
             * Добавлен тоггл ButtonsWasLoaded, для единичной загрузки кнопок
             */
            if (!ButtonsWasLoaded)
            {
                var buttons = ButtonFactory.GetButtons(SplitterObjectType, (ePackageElementType)GetValue(PackageElementTypeProperty));
                foreach (var item in buttons)
                {
                    item.Click += Button_Click;
                    mainStack.Children.Add(item);
                }
                ButtonsWasLoaded = true;
            }
        }
    }
}
