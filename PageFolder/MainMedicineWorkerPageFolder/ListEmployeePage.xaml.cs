using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using DiplomDolgov.WindowFolder.SharedWindowsFolder;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomDolgov.PageFolder.MainMedicineWorkerPageFolder
{
    /// <summary>
    /// Логика взаимодействия для ListEmployeePage.xaml
    /// </summary>
    public partial class ListEmployeePage : Page
    {
        private string searchText = string.Empty;
        private string selectedGender = string.Empty;
        private string selectedPost = string.Empty;

        public ListEmployeePage()
        {
            InitializeComponent();
            LoadStaff();
            LoadGenders();
            LoadPosts();
        }

        private void LoadStaff()
        {
            var staffList = DBEntities.GetContext().Staff.ToList();
            ListStaffLB.ItemsSource = staffList;
            FilterStaff();
        }

        private void LoadGenders()
        {
            var genders = DBEntities.GetContext().Gender.ToList();
            genders.Insert(0, new Gender { IdGender = 0, NameGender = "Все" });
            GenderCB.ItemsSource = genders;
        }

        private void LoadPosts()
        {
            var posts = DBEntities.GetContext().Post.ToList();
            posts.Insert(0, new Post { IdPost = 0, NamePost = "Все" });
            PostCB.ItemsSource = posts;
        }

        private void FilterStaff()
        {
            var filteredStaff = DBEntities.GetContext().Staff.ToList();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredStaff = filteredStaff
                    .Where(staff => staff.LastNameStaff.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    staff.FirstNameStaff.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    staff.MiddleNameStaff.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    staff.EmailStaff.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    staff.PhoneNumberStaff.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(selectedGender) && selectedGender != "Все")
            {
                filteredStaff = filteredStaff
                    .Where(staff => staff.Gender.NameGender == selectedGender)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(selectedPost) && selectedPost != "Все")
            {
                filteredStaff = filteredStaff
                    .Where(staff => staff.Post.NamePost == selectedPost)
                    .ToList();
            }

            ListStaffLB.ItemsSource = filteredStaff;
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedStaff = ListStaffLB.SelectedItem as Staff;

                if (selectedStaff == null)
                {
                    new MaterialDesignMessageBox("Выберите сотрудника для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                var staff = DBEntities.GetContext().Staff.FirstOrDefault(u => u.IdStaff == selectedStaff.IdStaff);

                if (staff == null)
                {
                    new MaterialDesignMessageBox("Сотрудник не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {staff.LastNameStaff}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

                if (result == true)
                {
                    DBEntities.GetContext().Staff.Remove(staff);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Сотрудник успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadStaff();
                    ListStaffLB.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"Ошибка: {ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void EditM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaff = ListStaffLB.SelectedItem as Staff;

            if (selectedStaff == null)
            {
                new MaterialDesignMessageBox("Выберите сотрудника для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var staff = DBEntities.GetContext().Staff.FirstOrDefault(u => u.IdStaff == selectedStaff.IdStaff);

            if (staff == null)
            {
                new MaterialDesignMessageBox("Сотрудник не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editStaffWindow = new EditStaffWindow(staff);

            var mainMedicineWorkerMainWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWorkerMainWindow != null)
            {
                // Показываем затемняющий слой
                mainMedicineWorkerMainWindow.ShowOverlay2();
                editStaffWindow.Closed += (s, args) => mainMedicineWorkerMainWindow.HideOverlay2();
            }

            editStaffWindow.ShowDialog();
            LoadStaff();
            ListStaffLB.Items.Refresh();
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = SearchTB.Text;
            FilterStaff();
        }

        private void ListStaffLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedStaff = ListStaffLB.SelectedItem as Staff;
            if (selectedStaff != null)
            {
                var detailsWindow = new StaffDetailsWindow(selectedStaff);
                detailsWindow.Show();
            }
        }

        private void GenderCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedGender = (GenderCB.SelectedItem as Gender)?.NameGender ?? string.Empty;
            FilterStaff();
        }

        private void PostCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPost = (PostCB.SelectedItem as Post)?.NamePost ?? string.Empty;
            FilterStaff();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addStaffWindow = new AddStaffWindow();

            var mainMedicineWorkerMainWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWorkerMainWindow != null)
            {
                // Показываем затемняющий слой
                mainMedicineWorkerMainWindow.ShowOverlay2();
                addStaffWindow.Closed += (s, args) => mainMedicineWorkerMainWindow.HideOverlay2();
            }

            // Подписываемся на событие AddedManufacturer, которое будет вызываться после успешного добавления производителя
            addStaffWindow.AddedStaff += AddStaffWindow_AddedStaff;

            addStaffWindow.ShowDialog();
        }

        private void AddStaffWindow_AddedStaff(object sender, EventArgs e)
        {
            LoadStaff();
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = FindParent<ScrollViewer>((DependencyObject)sender);
            if (scrollViewer != null)
            {
                scrollViewer.LineUp();
                e.Handled = true;
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
