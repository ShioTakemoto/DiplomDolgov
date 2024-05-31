using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    public partial class AddOrdersWindow : Window
    {
        public event EventHandler AddedOrder;
        public AddOrdersWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
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
                    AddedOrder?.Invoke(this, EventArgs.Empty);
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
            dateTime = DateTime.MinValue; // Присваиваем начальное значение

            // Получаем выбранную дату из DatePicker
            DateTime selectedDate = DatePicker.SelectedDate ?? DateTime.Now.Date;

            // Получаем время из текстового поля TimeTextBox
            string[] timeParts = TimeTextBox.Text.Split(':');
            if (timeParts.Length != 2 || !int.TryParse(timeParts[0], out int hours) || !int.TryParse(timeParts[1], out int minutes))
            {
                ShowErrorMessage("Неверный формат времени! Введите время в формате ЧЧ:ММ.");
                return false;
            }

            // Собираем дату и время в один объект DateTime
            dateTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, hours, minutes, 0);

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
