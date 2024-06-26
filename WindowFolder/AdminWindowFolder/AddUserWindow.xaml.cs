﻿using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace DiplomDolgov.WindowFolder.AdminWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public event EventHandler AddedUser;
        public AddUserWindow()
        {
            InitializeComponent();
            RoleCB.ItemsSource = DBEntities.GetContext().Role.ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (new MaterialDesignMessageBox("Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog() == true)
            {
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
            }
        }

        private void AddButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(LoginTB.Text))
                {
                    new MaterialDesignMessageBox("Введите логин!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (string.IsNullOrEmpty(PasswordPB.Password))
                {
                    new MaterialDesignMessageBox("Введите пароль!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (PasswordPB.Password != PasswordPB2.Password)
                {
                    new MaterialDesignMessageBox("Пароли не совпадают!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (RoleCB.SelectedIndex == -1)
                {
                    new MaterialDesignMessageBox("Вы не выбрали роль!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (DBEntities.GetContext().User.Any(u => u.LoginUser == LoginTB.Text))
                {
                    new MaterialDesignMessageBox("Пользователь с таким логином уже существует!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    LoginTB.Focus();
                }
                else
                {
                    var context = DBEntities.GetContext();
                    var user = new User
                    {
                        LoginUser = LoginTB.Text,
                        PasswordUser = PasswordPB.Password,
                        IdRole = Convert.ToInt32(RoleCB.SelectedValue)
                    };
                    context.User.Add(user);
                    context.SaveChanges();
                    new MaterialDesignMessageBox("Пользователь создан", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    ElementsToolsClass.ClearAllControls(this);
                    AddedUser?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"Ошибка: {ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
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

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
