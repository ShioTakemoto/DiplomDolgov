﻿using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DiplomDolgov.PageFolder.PharmacistPageFolder
{
    public partial class ListOrderPage : Page
    {
        public ListOrderPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var context = DBEntities.GetContext();
            var orderStatuses = context.OrderStatus.ToList();
            orderStatuses.Insert(0, new OrderStatus { IdOrderStatus = 0, NameOrderStatus = "Все" });
            OrderStatusCB.ItemsSource = orderStatuses;
            FilterOrders();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["PageFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void AddOrdersWindow_AddedOrder(object sender, EventArgs e)
        {
            // Обновляем DataGrid при получении события об успешном добавлении заказа
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
            var context = DBEntities.GetContext();
            var orders = context.Orders.AsQueryable();

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

                var context = DBEntities.GetContext();
                var order = context.Orders.FirstOrDefault(u => u.IdOrder == selectedOrder.IdOrder);

                if (order == null)
                {
                    new MaterialDesignMessageBox("Заказ не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {order.DateTimeOrder}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

                if (result == true)
                {
                    context.Orders.Remove(order);
                    context.SaveChanges();
                    new MaterialDesignMessageBox("Заказ успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadData();
                    ListOrderDG.Items.Refresh();
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

            var context = DBEntities.GetContext();
            var order = context.Orders.FirstOrDefault(u => u.IdOrder == selectedOrder.IdOrder);

            if (order == null)
            {
                new MaterialDesignMessageBox("Заказ не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editOrdersWindow = new EditOrdersWindow(order);

            var pharmacistWindow = Window.GetWindow(this) as PharmacistWindow;
            if (pharmacistWindow != null)
            {
                // Показываем затемняющий слой
                pharmacistWindow.ShowOverlay1();
                editOrdersWindow.Closed += (s, args) => pharmacistWindow.HideOverlay1();
            }
            editOrdersWindow.Topmost = true; // Устанавливаем на передний план
            editOrdersWindow.ShowDialog();
            editOrdersWindow.Activate(); // Активируем окно
            LoadData();
            ListOrderDG.Items.Refresh();
        }

        private void AddManufacturerBtn_Click(object sender, RoutedEventArgs e)
        {
            var addOrdersWindow = new AddOrdersWindow();

            var pharmacistWindow = Window.GetWindow(this) as PharmacistWindow;
            if (pharmacistWindow != null)
            {
                // Показываем затемняющий слой
                pharmacistWindow.ShowOverlay1();
                addOrdersWindow.Closed += (s, args) => pharmacistWindow.HideOverlay1();
            }

            // Подписываемся на событие AddedOrder, которое будет вызываться после успешного добавления заказа
            addOrdersWindow.AddedOrder += AddOrdersWindow_AddedOrder;

            addOrdersWindow.Topmost = true; // Устанавливаем на передний план
            addOrdersWindow.ShowDialog();
            addOrdersWindow.Activate(); // Активируем окно
        }
    }
}
