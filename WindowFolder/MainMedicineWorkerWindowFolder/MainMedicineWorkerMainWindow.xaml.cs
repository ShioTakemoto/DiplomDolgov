using DiplomDolgov.PageFolder.MainMedicineWorkerPageFolder;
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

namespace DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для MainMedicineWorkerMainWindow.xaml
    /// </summary>
    public partial class MainMedicineWorkerMainWindow : Window
    {
        public MainMedicineWorkerMainWindow()
        {
            InitializeComponent();
        }

        private void ListMedicineBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MainMedicineWorkerListMedicine());
        }

        private void ListEmployeeBtn_Click_1(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListEmployeePage());
        }

        private void ListGuestsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListGuestsPage());
        }

        private void AccountingBookBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result.Value)
            {
                this.Close();
            }
        }

        private void ListLittleTablesBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ShowOverlay2()
        {
            Overlay2.Visibility = Visibility.Visible;
        }

        public void HideOverlay2()
        {
            Overlay2.Visibility = Visibility.Collapsed;
        }
    }
}
