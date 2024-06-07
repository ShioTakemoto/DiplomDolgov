using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["PageFadeIn"];
            fadeInAnimation.Begin(this);
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
                 (r.Staff != null && r.Staff.LastNameStaff.ToLower().Contains(searchText)) ||
                 (r.Guests != null && r.Guests.LastNameGuest.ToLower().Contains(searchText)))
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

            bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {selectedRealization.DateTimeRealization}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
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

        private void AddRealizationWindow_AddedRealizationSuccess(object sender, EventArgs e)
        {
            LoadData();
        }

        private void ShowErrorMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();

        private void ShowSuccessMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();

        private void AddRealizBtn_Click(object sender, RoutedEventArgs e)
        {
            var addRealizationWindow = new AddRealizationWindow();

            var mainMedicineWorkerMainWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWorkerMainWindow != null)
            {
                mainMedicineWorkerMainWindow.ShowOverlay2();
                addRealizationWindow.Closed += (s, args) => mainMedicineWorkerMainWindow.HideOverlay2();
            }

            addRealizationWindow.AddedRealizationSuccess += AddRealizationWindow_AddedRealizationSuccess;
            addRealizationWindow.Topmost = true; // Устанавливаем на передний план
            addRealizationWindow.ShowDialog();
            addRealizationWindow.Activate(); // Активируем окно
        }
    }
}
