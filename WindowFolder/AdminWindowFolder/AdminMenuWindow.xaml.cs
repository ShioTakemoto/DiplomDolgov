using DiplomDolgov.ClassFolder;
using DiplomDolgov.PageFolder.AdminPageFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Media.Animation;
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
            MainFrame.Navigate(new ListUserPage());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
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
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CollapseAndUnfold_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }
    }
}
