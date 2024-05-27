using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    public partial class AddOrdersWindow : Window
    {
        public AddOrdersWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MedicineCB.ItemsSource = DBEntities.GetContext().Medicine.ToList();
            OrderStatusCB.ItemsSource = DBEntities.GetContext().OrderStatus.ToList();
        }

        private void AddButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ElementsToolsClass.AllFieldsFilled(this))
                {
                    if (!TryGetDateTime(out DateTime dateTimeOrder))
                        return;

                    if (!int.TryParse(CountTB.Text, out int count))
                    {
                        ShowErrorMessage("Неверный формат количества! Введите число.");
                        return;
                    }

                    var newOrder = new Orders
                    {
                        IdMedicine = Convert.ToInt32(MedicineCB.SelectedValue),
                        OrderDescription = OrderDescriptionTB.Text,
                        Count = count,
                        IdOrderStatus = Convert.ToInt32(OrderStatusCB.SelectedValue),
                        DateTimeOrder = dateTimeOrder
                    };

                    DBEntities.GetContext().Orders.Add(newOrder);
                    DBEntities.GetContext().SaveChanges();
                    ShowSuccessMessage("Заказ добавлен");
                    ElementsToolsClass.ClearAllControls(this);
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

        private bool TryGetDateTime(out DateTime dateTime)
        {
            if (!DateTime.TryParseExact($"{DatePicker.SelectedDate ?? DateTime.Now} {TimeTextBox.Text}", "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                ShowErrorMessage("Неверный формат времени! Введите время в формате ДД.ММ.ГГГГ ЧЧ:ММ.");
                return false;
            }
            return true;
        }

        private void ShowErrorMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();

        private void ShowSuccessMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (new MaterialDesignMessageBox("Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog() == true)
                Close();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    }
}
