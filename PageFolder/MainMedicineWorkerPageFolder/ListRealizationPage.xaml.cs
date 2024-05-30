using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomDolgov.PageFolder.MainMedicineWorkerPageFolder
{
    /// <summary>
    /// Логика взаимодействия для ListRealizationPage.xaml
    /// </summary>
        public partial class ListRealizationPage : Page
        {
        private List<Realization> allRealizations;
        private readonly DBEntities context;

        public ListRealizationPage()
        {
            InitializeComponent();
            context = DBEntities.GetContext();
            LoadData();
        }

        private void LoadData()
        {
                allRealizations = context.Realization.Include("Staff").Include("Guests").ToList();
                UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            ListRealizationDG.ItemsSource = allRealizations;
        }

        private void RealizDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            DateTime? selectedDate = RealizDatePicker.SelectedDate;
            string searchText = SearchTB.Text.Trim().ToLower();

            var filteredData = allRealizations.Where(r =>
                (selectedDate == null || r.DateTimeRealization.Date == selectedDate.Value.Date) &&
                (string.IsNullOrEmpty(searchText) ||
                 r.Staff.LastNameStaff.ToLower().Contains(searchText) ||
                 r.Guests.LastNameGuest.ToLower().Contains(searchText))
            ).ToList();

            ListRealizationDG.ItemsSource = filteredData;
        }

        private void EditM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedRealization = ListRealizationDG.SelectedItem as Realization;

            if (selectedRealization == null)
            {
                ShowErrorMessage("Выберите запись для редактирования!");
                return;
            }

            var editRealizationWindow = new EditRealizationWindow(selectedRealization);
            var mainMedicineWorkerMainWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;

            if (mainMedicineWorkerMainWindow != null)
            {
                mainMedicineWorkerMainWindow.ShowOverlay2();
                editRealizationWindow.Closed += (s, args) =>
                {
                    mainMedicineWorkerMainWindow.HideOverlay2();
                    LoadData();
                };
            }

            editRealizationWindow.ShowDialog();
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedRealization = ListRealizationDG.SelectedItem as Realization;

            if (selectedRealization == null)
            {
                ShowErrorMessage("Выберите запись для удаления!");
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить запись {selectedRealization.DateTimeRealization}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    context.Realization.Remove(selectedRealization);
                    context.SaveChanges();
                    ShowSuccessMessage("Запись успешно удалена.");
                    LoadData();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Ошибка при удалении записи: " + ex.Message);
                }
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addRealizationWindow = new AddRealizationWindow();

            var mainMedicineWorkerMainWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWorkerMainWindow != null)
            {
                mainMedicineWorkerMainWindow.ShowOverlay2();
                addRealizationWindow.Closed += (s, args) => mainMedicineWorkerMainWindow.HideOverlay2();
            }
            addRealizationWindow.AddedRealizationSuccess += AddRealizationWindow_AddedRealizationSuccess;

            addRealizationWindow.ShowDialog();
        }

        private void AddRealizationWindow_AddedRealizationSuccess(object sender, EventArgs e)
        {
            LoadData();
        }
        private void ShowErrorMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();

        private void ShowSuccessMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();
    }
}
