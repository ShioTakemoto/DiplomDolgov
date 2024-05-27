using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Linq;
using System.Windows;

namespace DiplomDolgov.WindowFolder.AdminWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
            RoleCB.ItemsSource = DBEntities.GetContext().Role.ToList();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (new MaterialDesignMessageBox("Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog() == true)
            {
                this.Close();
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
    }
}
