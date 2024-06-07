using DiplomDolgov.ClassFolder;
using DiplomDolgov.WindowFolder.AdminWindowFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.DataFolder;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;

namespace DiplomDolgov.WindowFolder
{
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MaterialDesignMessageBox("Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result.Value)
            {
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
            }
        }

        private void LoginButton(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTB.Text))
            {
                new MaterialDesignMessageBox("Введите логин", MessageType.Error, MessageButtons.Ok).ShowDialog();
                LoginTB.Focus();
            }
            else if (string.IsNullOrWhiteSpace(PasswordPB.Password))
            {
                new MaterialDesignMessageBox("Введите пароль", MessageType.Error, MessageButtons.Ok).ShowDialog();
                PasswordPB.Focus();
            }
            else
            {
                try
                {
                    var user = DBEntities.GetContext()
                        .User
                        .FirstOrDefault(u => u.LoginUser == LoginTB.Text);
                    if (user == null || user.PasswordUser != PasswordPB.Password)
                    {
                        new MaterialDesignMessageBox("Неверные данные", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }
                    else
                    {
                        Window nextWindow = null;
                        string welcomeMessage = "";

                        switch (user.IdRole)
                        {
                            case 1:
                                welcomeMessage = "Администратор, добро пожаловать";
                                nextWindow = new AdminMenuWindow();
                                break;
                            case 2:
                                welcomeMessage = "Фармацевт, добро пожаловать";
                                nextWindow = new PharmacistWindow();
                                break;
                            case 3:
                                welcomeMessage = "Главный мед. работник, добро пожаловать";
                                nextWindow = new MainMedicineWorkerMainWindow();
                                break;
                        }

                        if (nextWindow != null)
                        {
                            new MaterialDesignMessageBox(welcomeMessage, MessageType.Info, MessageButtons.Ok).ShowDialog();
                            nextWindow.Closed += (s, args) => this.Show();
                            this.Hide();
                            nextWindow.Show();
                        }
                    }
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new MaterialDesignMessageBox("Обратитесь к администратору", MessageType.Info, MessageButtons.Ok).ShowDialog();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CollapseAndUnfold_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Topmost = true;
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.CanResize;
                Topmost = false;
            }
        }
    }
}