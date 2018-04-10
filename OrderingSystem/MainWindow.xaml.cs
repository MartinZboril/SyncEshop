﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OrderingSystem.DataService;

namespace OrderingSystem
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Sync();
        }

        private async void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            syncService syncservice = new syncService();
            await syncservice.SyncAsync();
            _mainFrame.Refresh();
        }

        private async void Sync()
        {
            syncService syncservice = new syncService();
            await syncservice.SyncAsync();
            _mainFrame.Refresh();
        }
    }
}
