using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для AddRealizationWindow.xaml
    /// </summary>
    public partial class AddRealizationWindow : Window
    {
        public List<Guests> Guests { get; set; }
        public List<Staff> Staff { get; set; }
        public event EventHandler AddedRealizationSuccess;
        public AddRealizationWindow()
        {
            InitializeComponent();
            LoadMedicines();
            Guests = DBEntities.GetContext().Guests.ToList();
            Staff = DBEntities.GetContext().Staff.ToList();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMedicines();
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadMedicines()
        {
            MedicineCB.ItemsSource = DBEntities.GetContext().Medicine.ToList();
            Guests = DBEntities.GetContext().Guests.ToList(); // Инициализация списка гостей
            Staff = DBEntities.GetContext().Staff.ToList(); // Инициализация списка сотрудников
        }

        private void AddButton(object sender, RoutedEventArgs e)
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
                var selectedMedicine = DBEntities.GetContext().Medicine.FirstOrDefault(m => m.IdMedicine == selectedMedicineId);

                if (selectedMedicine == null)
                {
                    ShowErrorMessage("Медикамент не найден.");
                    return;
                }

                // Поиск записи в таблице StockUnit по IdMedicine
                var stockUnit = selectedMedicine.StockUnit.FirstOrDefault(su => su.IdMedicine == selectedMedicineId);

                if (stockUnit == null)
                {
                    // Если запись не найдена, обработать эту ситуацию
                    ShowErrorMessage("Данные о запасах для выбранного медикамента отсутствуют.");
                    return;
                }

                // Проверка доступного количества
                if (count > stockUnit.Count)
                {
                    ShowErrorMessage($"Недостаточное количество медикамента на складе. Доступно: {stockUnit.Count}");
                    return;
                }

                var newRealization = new Realization
                {
                    IdMedicine = selectedMedicineId,
                    Reason = ReasonTB.Text,
                    DateTimeRealization = dateTimeRealization,
                    Count = count
                };

                if (GuestCB.SelectedIndex >= 0)
                {
                    newRealization.IdGuest = Convert.ToInt32(GuestCB.SelectedValue);
                }

                if (StaffCB.SelectedIndex >= 0)
                {
                    newRealization.IdStaff = Convert.ToInt32(StaffCB.SelectedValue);
                }

                DBEntities.GetContext().Realization.Add(newRealization);
                DBEntities.GetContext().SaveChanges();

                AddedRealizationSuccess?.Invoke(this, EventArgs.Empty);
                ShowSuccessMessage("Запись добавлена");
                ClearAllControls();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка: " + ex.Message);
            }
        }

        private void ClearAllControls()
        {
            StaffCB.SelectedIndex = -1;
            GuestCB.SelectedIndex = -1;
            MedicineCB.SelectedIndex = -1;
            ReasonTB.Clear();
            CountTB.Clear();
            DatePicker.SelectedDate = null;
            TimeTextBox.Clear();
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