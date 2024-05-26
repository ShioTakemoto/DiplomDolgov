using DiplomDolgov.DataFolder;
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
            RoleCB.ItemsSource = DBEntities.GetContext().Role.ToList();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result == true)
            {
                this.Close();
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(LoginTB.Text))
                {
                    new MaterialDesignMessageBox("Введите логин", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (string.IsNullOrEmpty(PasswordPB.Password))
                {
                    new MaterialDesignMessageBox("Введите пароль", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (RoleCB.SelectedIndex == -1)
                {
                    new MaterialDesignMessageBox("Вы не выбрали роль", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else
                {
                    var userToUpdate = DBEntities.GetContext().User.FirstOrDefault(u => u.IdUser == user.IdUser);

                    if (userToUpdate != null)
                    {
                        userToUpdate.LoginUser = LoginTB.Text;
                        userToUpdate.PasswordUser = PasswordPB.Password;
                        userToUpdate.IdRole = (int)RoleCB.SelectedValue;

                        DBEntities.GetContext().SaveChanges();
                        new MaterialDesignMessageBox("Данные успешно сохранены", MessageType.Success, MessageButtons.Ok).ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        new MaterialDesignMessageBox("Пользователь не найден", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
