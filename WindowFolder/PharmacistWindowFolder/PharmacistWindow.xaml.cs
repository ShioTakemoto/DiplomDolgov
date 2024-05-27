﻿using DiplomDolgov.PageFolder.AdminPageFolder;
using DiplomDolgov.PageFolder.PharmacistPageFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
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
using System.Windows.Shapes;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для PharmacistWindow.xaml
    /// </summary>
    public partial class PharmacistWindow : Window
    {
        public PharmacistWindow()
        {
            InitializeComponent();
        }

        private void ListMedicineBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PharmacistListMedicinePage());
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result.Value)
            {
                this.Close();
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ListManufacturersBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListManufacturerPage());
        }

        private void ListOrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListOrderPage());
        }

        private void ListLittleTablesBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AnotherTablesPharmacistPage());
        }
    }
}
