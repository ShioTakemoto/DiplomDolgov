using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    public partial class EditOrdersWindow : Window
    {
        private Orders order;

        public EditOrdersWindow(Orders order)
        {
            InitializeComponent();
            InitializeComboBoxes();
            this.order = order;
            DataContext = this.order;
        }

        private void InitializeComboBoxes()
        {
            MedicineCB.ItemsSource = DBEntities.GetContext().Medicine.ToList();
            OrderStatusCB.ItemsSource = DBEntities.GetContext().OrderStatus.ToList();
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ElementsToolsClass.AllFieldsFilled(this))
                {
                    if (!TryParseDateTime(out DateTime dateTimeOrder))
                        return;

                    if (!int.TryParse(CountTB.Text, out int count))
                    {
                        ShowErrorMessage("Неверный формат количества! Введите число.");
                        return;
                    }

                    DBEntities.GetContext().SaveChanges();
                    ShowSuccessMessage("Данные успешно сохранены");
                    Close();
                }
                else
                {
                    ShowErrorMessage("Не все поля заполнены!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.ToString());
            }
        }

        private bool TryParseDateTime(out DateTime result)
        {
            result = DateTime.Now;
            if (DatePicker.SelectedDate == null || !TimeSpan.TryParseExact(TimeTextBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan selectedTime))
            {
                ShowErrorMessage("Неверный формат времени! Введите время в формате ЧЧ:ММ.");
                return false;
            }

            result = DatePicker.SelectedDate.Value.Date + selectedTime;
            return true;
        }

        private void ShowErrorMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();
        }

        private void ShowSuccessMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
