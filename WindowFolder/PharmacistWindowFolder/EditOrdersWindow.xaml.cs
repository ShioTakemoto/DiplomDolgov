using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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

            Loaded += EditOrdersWindow_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void EditOrdersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем значения DatePicker и TimeTextBox после загрузки окна
            DatePicker.SelectedDate = order.DateTimeOrder.Date;
            TimeTextBox.Text = order.DateTimeOrder.ToString("HH:mm");
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

                    // Обновляем свойства объекта заказа перед сохранением изменений
                    order.DateTimeOrder = dateTimeOrder;
                    order.Count = count;

                    // Обновляем привязки данных для всех полей
                    OrderDescriptionTB.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    CountTB.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    MedicineCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                    OrderStatusCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                    DatePicker.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateSource();
                    TimeTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

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
                ShowErrorMessage("Неверный формат времени!\nВведите время в формате ЧЧ:ММ.");
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
            bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
