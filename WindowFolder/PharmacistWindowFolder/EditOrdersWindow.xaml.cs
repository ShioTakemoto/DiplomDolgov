using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для EditOrdersWindow.xaml
    /// </summary>
    public partial class EditOrdersWindow : Window
    {
        private Orders order;
        public EditOrdersWindow(Orders order)
        {
            InitializeComponent();
            MedicineCB.ItemsSource = DBEntities.GetContext().Medicine.ToList();
            OrderStatusCB.ItemsSource = DBEntities.GetContext().OrderStatus.ToList();
            this.order = order;
            DataContext = this.order;

            // Установка начальных значений для DatePicker и TimeTextBox
            DatePicker.SelectedDate = order.DateTimeOrder.Date;
            TimeTextBox.Text = order.DateTimeOrder.ToString("hh\\:mm");
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ElementsToolsClass.AllFieldsFilled(this))
                {
                    DateTime selectedDate = DatePicker.SelectedDate ?? DateTime.Now;
                    TimeSpan selectedTime;

                    if (!TimeSpan.TryParseExact(TimeTextBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out selectedTime))
                    {
                        new MaterialDesignMessageBox("Неверный формат времени! Введите время в формате ЧЧ:ММ.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    DateTime dateTimeOrder = selectedDate.Date + selectedTime;

                    int idMedicine = Convert.ToInt32(MedicineCB.SelectedValue);
                    int idOrderStatus = Convert.ToInt32(OrderStatusCB.SelectedValue);
                    int count;

                    if (!int.TryParse(CountTB.Text, out count))
                    {
                        new MaterialDesignMessageBox("Неверный формат количества! Введите число.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    order.IdMedicine = idMedicine;
                    order.OrderDescription = OrderDescriptionTB.Text;
                    order.Count = count;
                    order.IdOrderStatus = idOrderStatus;
                    order.DateTimeOrder = dateTimeOrder;

                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Данные успешно сохранены", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    this.Close();
                }
                else
                {
                    new MaterialDesignMessageBox("Не все поля заполнены!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
