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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для EditRealizationWindow.xaml
    /// </summary>
    public partial class EditRealizationWindow : Window
    {
        private Realization realization;
        private DBEntities _context;

        public EditRealizationWindow(Realization realization)
        {
            InitializeComponent();
            this.realization = realization;
            DataContext = this.realization;
            _context = DBEntities.GetContext();

            Loaded += EditRealizationWindow_Loaded; // Добавляем обработчик события загрузки окна
            Loaded += EditRealization1Window_Loaded;
        }

        private void EditRealization1Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем значения DatePicker и TimeTextBox после загрузки окна
            DatePicker.SelectedDate = realization.DateTimeRealization.Date;
            TimeTextBox.Text = realization.DateTimeRealization.ToString("HH:mm");
        }

        private void EditRealizationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GuestCB.ItemsSource = _context.Guests.ToList();
            StaffCB.ItemsSource = _context.Staff.ToList();
            MedicineCB.ItemsSource = _context.Medicine.ToList();

            GuestCB.SelectedItem = _context.Guests.FirstOrDefault(guest => guest.IdGuest == realization.IdGuest);
            StaffCB.SelectedItem = _context.Staff.FirstOrDefault(staff => staff.IdStaff == realization.IdStaff);
            MedicineCB.SelectedItem = _context.Medicine.FirstOrDefault(med => med.IdMedicine == realization.IdMedicine);
        }

        private void ShowErrorMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();

        private void ShowSuccessMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (new MaterialDesignMessageBox("Вы уверены, что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog() == true)
                Close();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

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

                if (!TryParseDateTime(out DateTime dateTimeRealization))
                    return;

                if (GuestCB.SelectedIndex < 0 && StaffCB.SelectedIndex < 0)
                {
                    ShowErrorMessage("Выберите либо гостя, либо сотрудника.");
                    return;
                }

                if (GuestCB.SelectedIndex >= 0 && StaffCB.SelectedIndex >= 0)
                {
                    ShowErrorMessage("Нельзя выбрать одновременно и гостя, и сотрудника.");
                    return;
                }

                int selectedMedicineId = Convert.ToInt32(MedicineCB.SelectedValue);
                var selectedMedicine = _context.StockUnit.FirstOrDefault(m => m.IdMedicine == selectedMedicineId);

                if (selectedMedicine == null || count > selectedMedicine.Count)
                {
                    ShowErrorMessage("Указанное количество превышает доступное на складе.");
                    return;
                }

                realization.IdMedicine = selectedMedicineId;
                realization.Reason = ReasonTB.Text;
                realization.DateTimeRealization = dateTimeRealization;
                realization.Count = count;

                if (GuestCB.SelectedIndex >= 0)
                {
                    realization.IdGuest = Convert.ToInt32(GuestCB.SelectedValue);
                }
                else
                {
                    realization.IdGuest = null;
                }

                if (StaffCB.SelectedIndex >= 0)
                {
                    realization.IdStaff = Convert.ToInt32(StaffCB.SelectedValue);
                }
                else
                {
                    realization.IdStaff = null;
                }

                _context.SaveChanges();
                ShowSuccessMessage("Данные успешно сохранены");
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка при сохранении данных: " + ex.Message);
            }
        }

        private bool TryParseDateTime(out DateTime result)
        {
            result = DateTime.Now;
            if (DatePicker.SelectedDate == null || !TimeSpan.TryParseExact(TimeTextBox.Text, "hh\\:mm", System.Globalization.CultureInfo.InvariantCulture, out TimeSpan selectedTime))
            {
                ShowErrorMessage("Неверный формат времени! Введите время в формате ЧЧ:ММ.");
                return false;
            }

            result = DatePicker.SelectedDate.Value.Date + selectedTime;
            return true;
        }

        private void ClearGuest(object sender, RoutedEventArgs e)
        {
            GuestCB.SelectedIndex = -1;
        }

        private void ClearStaff(object sender, RoutedEventArgs e)
        {
            StaffCB.SelectedIndex = -1;
        }
    }
}
