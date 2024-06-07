using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.AdminWindowFolder;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomDolgov.PageFolder.AdminPageFolder
{
    /// <summary>
    /// Логика взаимодействия для ListUserPage.xaml
    /// </summary>

    public partial class ListUserPage : Page
    {
        private bool passwordsVisible = false;
        private int? selectedRoleId = null;

        public ListUserPage()
        {
            InitializeComponent();
            Loaded += ListUserPage_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["PageFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void ListUserPage_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid();
            RoleCB.ItemsSource = DBEntities.GetContext().Role.ToList();
        }

        private void RefreshDataGrid()
        {
            var users = DBEntities.GetContext().User.ToList();

            if (selectedRoleId.HasValue)
            {
                users = users.Where(u => u.IdRole == selectedRoleId.Value).ToList();
            }

            var displayUsers = users.OrderBy(u => u.LoginUser).Select(u => new DisplayUser
            {
                LoginUser = u.LoginUser,
                PasswordUser = passwordsVisible ? u.PasswordUser : "*****",
                NameRole = u.Role.NameRole
            }).ToList();

            ListUserDG.ItemsSource = displayUsers;
        }

        private void TogglePasswords_Click(object sender, RoutedEventArgs e)
        {
            passwordsVisible = !passwordsVisible;
            RefreshDataGrid();
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = ListUserDG.SelectedItem as DisplayUser;

            if (selectedUser == null)
            {
                new MaterialDesignMessageBox("Выберите пользователя для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var user = DBEntities.GetContext().User.FirstOrDefault(u => u.LoginUser == selectedUser.LoginUser);

            if (user == null)
            {
                new MaterialDesignMessageBox("Пользователь не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {user.LoginUser}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                DBEntities.GetContext().User.Remove(user);
                DBEntities.GetContext().SaveChanges();
                new MaterialDesignMessageBox("Пользователь успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                RefreshDataGrid();
                ListUserDG.Items.Refresh();
            }
        }

        private void EditM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = ListUserDG.SelectedItem as DisplayUser;

            if (selectedUser == null)
            {
                new MaterialDesignMessageBox("Выберите пользователя для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var user = DBEntities.GetContext().User.FirstOrDefault(u => u.LoginUser == selectedUser.LoginUser);

            if (user == null)
            {
                new MaterialDesignMessageBox("Пользователь не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editUserWindow = new EditUserWindow(user);

            // Получаем ссылку на главное окно
            var adminWindow = Window.GetWindow(this) as AdminMenuWindow;
            if (adminWindow != null)
            {
                // Показываем затемняющий слой
                adminWindow.ShowOverlay();
                editUserWindow.Closed += (s, args) => adminWindow.HideOverlay();
            }

            editUserWindow.ShowDialog();
            RefreshDataGrid();
            ListUserDG.Items.Refresh();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var users = DBEntities.GetContext().User
                    .Where(p => p.LoginUser.Contains(SearchTB.Text) || p.PasswordUser.Contains(SearchTB.Text) || p.Role.NameRole.Contains(SearchTB.Text))
                    .ToList();

                if (selectedRoleId.HasValue)
                {
                    users = users.Where(u => u.IdRole == selectedRoleId.Value).ToList();
                }

                var displayUsers = users.OrderBy(p => p.LoginUser).Select(u => new DisplayUser
                {
                    LoginUser = u.LoginUser,
                    PasswordUser = passwordsVisible ? u.PasswordUser : "*****",
                    NameRole = u.Role.NameRole
                }).ToList();

                ListUserDG.ItemsSource = displayUsers;
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void RoleCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleCB.SelectedItem is Role selectedRole)
            {
                selectedRoleId = selectedRole.IdRole;
            }
            else
            {
                selectedRoleId = null;
            }
            RefreshDataGrid();
        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserWindow();

            // Получаем ссылку на главное окно
            var adminWindow = Window.GetWindow(this) as AdminMenuWindow;
            if (adminWindow != null)
            {
                // Показываем затемняющий слой
                adminWindow.ShowOverlay();
                addUserWindow.Closed += (s, args) => adminWindow.HideOverlay();
            }

            addUserWindow.AddedUser += AddUserWindow_AddedUser;
            addUserWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addUserWindow.Topmost = true; // Устанавливаем на передний план
            addUserWindow.ShowDialog();
            addUserWindow.Activate(); // Активируем окно
        }

        private void AddUserWindow_AddedUser(object sender, EventArgs e)
        {
            // Обновляем DataGrid при получении события об успешном добавлении пользователя
            RefreshDataGrid();
        }
    }
}