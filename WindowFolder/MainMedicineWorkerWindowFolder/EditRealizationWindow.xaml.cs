using DiplomDolgov.ClassFolder;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для EditRealizationWindow.xaml
    /// </summary>
    public partial class EditRealizationWindow : Window
    {
        public List<Guests> GuestsWithEmpty { get; set; }
        public List<Staff> StaffWithEmpty { get; set; }
        private Realization currentRealization;

        public EditRealizationWindow(Realization realization)
        {
            InitializeComponent();
            currentRealization = realization;
            LoadData();
            DataContext = currentRealization;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadData()
        {
            var context = DBEntities.GetContext();

            GuestsWithEmpty = new List<Guests> { new Guests { IdGuest = -1, LastNameGuest = "", FirstNameGuest = "", MiddleNameGuest = "" } }
                .Concat(context.Guests.ToList()).ToList();

            StaffWithEmpty = new List<Staff> { new Staff { IdStaff = -1, LastNameStaff = "", FirstNameStaff = "", MiddleNameStaff = "" } }
                .Concat(context.Staff.ToList()).ToList();

            MedicineCB.ItemsSource = context.Medicine.ToList();
            GuestCB.ItemsSource = GuestsWithEmpty;
            StaffCB.ItemsSource = StaffWithEmpty;
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MedicineCB.SelectedValue == null)
                {
                    ShowErrorMessage("Выберите медикамент.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ReasonTB.Text))
                {
                    ShowErrorMessage("Введите причину отпуска.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CountTB.Text) || !int.TryParse(CountTB.Text, out int count))
                {
                    ShowErrorMessage("Введите корректное количество.");
                    return;
                }

                if (!TryGetDateTime(out DateTime dateTimeRealization))
                    return;

                bool isGuestSelected = GuestCB.SelectedValue != null && (int)GuestCB.SelectedValue != -1;
                bool isStaffSelected = StaffCB.SelectedValue != null && (int)StaffCB.SelectedValue != -1;

                if (!isGuestSelected && !isStaffSelected)
                {
                    ShowErrorMessage("Выберите либо гостя, либо сотрудника.");
                    return;
                }

                if (isGuestSelected && isStaffSelected)
                {
                    ShowErrorMessage("Нельзя выбрать одновременно и гостя, и сотрудника.");
                    return;
                }

                int selectedMedicineId = Convert.ToInt32(MedicineCB.SelectedValue);
                var selectedMedicine = DBEntities.GetContext().Medicine.FirstOrDefault(m => m.IdMedicine == selectedMedicineId);

                if (selectedMedicine == null)
                {
                    ShowErrorMessage("Медикамент не найден.");
                    return;
                }

                var stockUnit = selectedMedicine.StockUnit.FirstOrDefault(su => su.IdMedicine == selectedMedicineId);

                if (stockUnit == null)
                {
                    ShowErrorMessage("Данные о запасах для выбранного медикамента отсутствуют.");
                    return;
                }

                if (count > stockUnit.Count)
                {
                    ShowErrorMessage($"Недостаточное количество медикамента на складе. Доступно: {stockUnit.Count}");
                    return;
                }

                currentRealization.IdMedicine = selectedMedicineId;
                currentRealization.Reason = ReasonTB.Text;
                currentRealization.DateTimeRealization = dateTimeRealization;
                currentRealization.Count = count;

                if (isGuestSelected)
                {
                    currentRealization.IdGuest = Convert.ToInt32(GuestCB.SelectedValue);
                    currentRealization.IdStaff = null;
                }
                else if (isStaffSelected)
                {
                    currentRealization.IdStaff = Convert.ToInt32(StaffCB.SelectedValue);
                    currentRealization.IdGuest = null;
                }

                DBEntities.GetContext().SaveChanges();
                ShowSuccessMessage("Запись обновлена.");
                this.Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка: " + ex.Message);
            }
        }

        private bool TryGetDateTime(out DateTime dateTime)
        {
            dateTime = DateTime.MinValue;
            if (!DatePicker.SelectedDate.HasValue)
            {
                ShowErrorMessage("Выберите дату реализации.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TimeTextBox.Text))
            {
                ShowErrorMessage("Введите время реализации.");
                return false;
            }

            string date = DatePicker.SelectedDate.Value.ToShortDateString();
            string dateTimeString = $"{date} {TimeTextBox.Text}";

            if (!DateTime.TryParse(dateTimeString, out dateTime))
            {
                ShowErrorMessage("Введите корректные дату и время реализации.");
                return false;
            }

            return true;
        }

        private void ShowErrorMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();

        private void ShowSuccessMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (new MaterialDesignMessageBox("Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog() == true)
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    }
}
