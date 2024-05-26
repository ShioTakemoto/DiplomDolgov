using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
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

namespace DiplomDolgov.PageFolder.PharmacistPageFolder
{
    /// <summary>
    /// Логика взаимодействия для ListOrderPage.xaml
    /// </summary>
    public partial class ListOrderPage : Page
    {
        public ListOrderPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var orderStatuses = DBEntities.GetContext().OrderStatus.ToList();
            orderStatuses.Insert(0, new OrderStatus { IdOrderStatus = 0, NameOrderStatus = "" }); // Добавляем пустое значение
            OrderStatusCB.ItemsSource = orderStatuses;

            FilterOrders();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadData();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterOrders();
        }

        private void OrderStatusCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterOrders();
        }

        private void FilterOrders()
        {
            var orders = DBEntities.GetContext().Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTB.Text))
            {
                orders = orders.Where(o => o.Medicine.NameMedicine.Contains(SearchTB.Text));
            }

            if (OrderStatusCB.SelectedItem is OrderStatus selectedStatus && selectedStatus.IdOrderStatus != 0)
            {
                orders = orders.Where(o => o.OrderStatus.IdOrderStatus == selectedStatus.IdOrderStatus);
            }

            ListOrderDG.ItemsSource = orders.ToList();
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedOrder = ListOrderDG.SelectedItem as Orders;

                if (selectedOrder == null)
                {
                    new MaterialDesignMessageBox("Выберите заказ для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                using (var context = DBEntities.GetContext())
                {
                    var order = context.Orders.FirstOrDefault(u => u.IdOrder == selectedOrder.IdOrder);

                    if (order == null)
                    {
                        new MaterialDesignMessageBox("Заказ не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {order.DateTimeOrder}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

                    if (result == true)
                    {
                        DBEntities.GetContext().Orders.Remove(order);
                        DBEntities.GetContext().SaveChanges();
                        new MaterialDesignMessageBox("Заказ успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                        LoadData();
                        ListOrderDG.Items.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
            
        }

        private void EditM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = ListOrderDG.SelectedItem as Orders;

            if (selectedOrder == null)
            {
                new MaterialDesignMessageBox("Выберите заказ для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var order = DBEntities.GetContext().Orders.FirstOrDefault(u => u.IdOrder == selectedOrder.IdOrder);

            if (order == null)
            {
                new MaterialDesignMessageBox("Заказ не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            new EditOrdersWindow(order).ShowDialog();
            LoadData();
            ListOrderDG.Items.Refresh();
        }
    }
}
