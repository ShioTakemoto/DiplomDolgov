using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
            LoadRoles();
            this.user = user;
            DataContext = this.user;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadRoles()
        {
            RoleCB.ItemsSource = DBEntities.GetContext().Role.ToList();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowConfirmationMessage("Вы уверены что хотите выйти?"))
            {
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
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
                    LoginTB.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    PasswordPB.GetBindingExpression(PasswordBoxHelper.BoundPasswordProperty)?.UpdateSource();
                    RoleCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
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
