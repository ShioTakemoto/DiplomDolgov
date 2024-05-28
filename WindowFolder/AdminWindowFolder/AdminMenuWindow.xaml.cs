using DiplomDolgov.PageFolder.AdminPageFolder;
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

namespace DiplomDolgov.WindowFolder.AdminWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AdminMenuWindow.xaml
    /// </summary>
    public partial class AdminMenuWindow : Window
    {
        public AdminMenuWindow()
        {
            InitializeComponent();
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

        private void ListUserBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListUserPage());
        }

        public void ShowOverlay()
        {
            Overlay.Visibility = Visibility.Visible;
        }

        public void HideOverlay()
        {
            Overlay.Visibility = Visibility.Collapsed;
        }
    }
}
