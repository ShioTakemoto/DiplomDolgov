﻿using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Linq;
using System.Windows;

namespace DiplomDolgov.WindowFolder.AdminWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        private User user;

        public EditUserWindow(User user)
        {
            InitializeComponent();
            this.user = user;
            DataContext = this.user;
            LoadRoles();
        }

        private void LoadRoles()
        {
            RoleCB.ItemsSource = DBEntities.GetContext().Role.ToList();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowConfirmationMessage("Вы уверены что хотите выйти?"))
            {
                Close();
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(LoginTB.Text))
                {
                    ShowErrorMessage("Введите логин");
                }
                else if (string.IsNullOrEmpty(PasswordPB.Password))
                {
                    ShowErrorMessage("Введите пароль");
                }
                else if (RoleCB.SelectedIndex == -1)
                {
                    ShowErrorMessage("Вы не выбрали роль");
                }
                else
                {
                    UpdateUser();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Произошла ошибка: {ex.Message}");
            }
        }

        private void UpdateUser()
        {
            var userToUpdate = DBEntities.GetContext().User.FirstOrDefault(u => u.IdUser == user.IdUser);
            if (userToUpdate != null)
            {
                DBEntities.GetContext().SaveChanges();
                ShowSuccessMessage("Данные успешно сохранены");
                Close();
            }
            else
            {
                ShowErrorMessage("Пользователь не найден");
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private bool ShowConfirmationMessage(string message)
        {
            bool? result = new MaterialDesignMessageBox(message, MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
            return result ?? false;
        }

        private void ShowErrorMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();
        }

        private void ShowSuccessMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();
        }
    }
}
