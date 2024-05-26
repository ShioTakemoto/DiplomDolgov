using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
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
            bool? Result = new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result.Value)
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
                else if (DBEntities.GetContext()
                            .User
                            .FirstOrDefault(
                    u => u.LoginUser == LoginTB.Text) != null)
                {
                    new MaterialDesignMessageBox("Пользователь с таким логином уже существует!", MessageType.Error, MessageButtons.Ok).ShowDialog();

                    LoginTB.Focus();

                }
                else
                {
                    var context = DBEntities.GetContext();
                    User user = new User();
                    user.LoginUser = LoginTB.Text;
                    user.PasswordUser = PasswordPB.Password;
                    user.IdRole = Int32.Parse
                        (RoleCB.SelectedValue.ToString());
                    context.User.Add(user);
                    context.SaveChanges();
                    new MaterialDesignMessageBox("Пользователь создан", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    ElementsToolsClass.ClearAllControls(this);

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
