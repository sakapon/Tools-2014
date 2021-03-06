﻿using System;
using System.Collections.Generic;
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

namespace PortraitClip
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = (MainViewModel)DataContext;

            CloseButton.Click += (o, e) =>
            {
                Close();
            };

            Closing += (o, e) =>
            {
                vm.Portrait.Dispose();
            };

            MouseLeftButtonDown += (o, e) =>
            {
                DragMove();
            };

            MouseRightButtonUp += (o, e) =>
            {
                vm.IsSettingMode = !vm.IsSettingMode;
            };
        }
    }
}
