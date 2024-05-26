using DiplomDolgov.ClassFolder;
using DiplomDolgov.WindowFolder.AdminWindowFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.DataFolder;
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
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;

namespace DiplomDolgov.WindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result.Value)
            {
                this.Close();
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
                        switch (user.IdRole)
                        {
                            case 1:
                                var admin = DBEntities.GetContext()
                                    .User
                                    .FirstOrDefault(c => c.LoginUser == user.LoginUser);
                                new MaterialDesignMessageBox("Администратор, добро пожаловать", MessageType.Info, MessageButtons.Ok).ShowDialog();
                                new AdminMenuWindow().Show();
                                break;
                            case 2:
                                var pharmacist = DBEntities.GetContext()
                                    .User
                                    .FirstOrDefault(c => c.LoginUser == user.LoginUser);
                                new MaterialDesignMessageBox("Фармацевт, добро пожаловать", MessageType.Info, MessageButtons.Ok).ShowDialog();
                                new PharmacistWindow().Show();
                                break;
                            case 3:
                                var mainmedicine = DBEntities.GetContext()
                                    .User
                                    .FirstOrDefault(c => c.LoginUser == user.LoginUser);
                                new MaterialDesignMessageBox("Главный медицинский работник, добро пожаловать", MessageType.Info, MessageButtons.Ok).ShowDialog();
                                new MainMedicineWorkerMainWindow().Show();
                                break;
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
    }
}
