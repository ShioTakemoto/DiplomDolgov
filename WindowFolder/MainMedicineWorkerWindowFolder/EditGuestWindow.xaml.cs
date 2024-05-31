using DiplomDolgov.ClassFolder;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для EditGuestWindow.xaml
    /// </summary>
    public partial class EditGuestWindow : Window
    {
        private Guests guest;

        public EditGuestWindow(Guests guest)
        {
            InitializeComponent();
            this.guest = guest;
            DataContext = this.guest;
            LoadRooms();
            LoadGenders();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadRooms()
        {
            RoomCB.ItemsSource = DBEntities.GetContext().Room.ToList();
        }

        private void LoadGenders()
        {
            GenderCB.ItemsSource = DBEntities.GetContext().Gender.ToList();
        }

        private void ShowErrorMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();
        }

        private void ShowWarningMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Warning, MessageButtons.Ok).ShowDialog();
        }

        private void ShowSuccessMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();
        }

        private bool ShowConfirmationMessage(string message)
        {
            bool? result = new MaterialDesignMessageBox(message, MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
            return result ?? false;
        }

        private int GetSelectedItemId<T>(ComboBox comboBox) where T : class
        {
            var selectedItem = comboBox.SelectedItem;
            if (selectedItem != null)
            {
                return (int)selectedItem.GetType().GetProperty($"Id{typeof(T).Name}").GetValue(selectedItem);
            }
            return -1;
        }

        private void AddRoomNumber(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новый номер комнаты");
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                else
                {
                    if (RoomExists(inputText))
                    {
                        ShowErrorMessage($"Комната с номером {inputText} уже существует!");
                    }
                    else
                    {
                        // Создаем новый объект комнаты
                        var newRoom = new Room { RoomNumber = inputText };

                        // Добавляем новую комнату в базу данных
                        var context = DBEntities.GetContext();
                        context.Room.Add(newRoom);
                        context.SaveChanges(); // Сохраняем изменения в базе данных

                        // Добавляем новый номер комнаты в ComboBox
                        AddComboBoxItem(RoomCB, newRoom);
                    }
                }
            }
        }

        private void RefreshComboBoxItems(ComboBox comboBox)
        {
            // Сохраняем текущий выбранный элемент
            var selectedItem = comboBox.SelectedItem;
            // Обнуляем источник данных
            comboBox.ItemsSource = null;
            // Получаем данные из базы данных
            var context = DBEntities.GetContext();
            var rooms = context.Room.ToList();
            // Устанавливаем новый источник данных
            comboBox.ItemsSource = rooms;
            // Восстанавливаем выбранный элемент
            comboBox.SelectedItem = selectedItem;
        }

        private bool RoomExists(string roomNumber)
        {
            var context = DBEntities.GetContext();
            var dbSet = context.Set<Room>();
            return dbSet.Any(room => room.RoomNumber == roomNumber);
        }

        private bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }

        private void AddComboBoxItem<T>(ComboBox comboBox, T newItem)
        {
            // Проверяем, что newItem не равен null
            if (newItem != null)
            {
                // Получаем текущий источник данных
                var itemsSource = comboBox.ItemsSource as IList<T>;

                // Если источник данных равен null, создаем новый список
                if (itemsSource == null)
                {
                    itemsSource = new List<T>();
                }

                // Добавляем новый элемент в источник данных
                itemsSource.Add(newItem);

                // Устанавливаем обновленный список как источник данных ComboBox
                comboBox.ItemsSource = itemsSource;

                // Устанавливаем выбранным новый элемент
                comboBox.SelectedItem = newItem;
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowConfirmationMessage("Вы уверены, что хотите выйти?"))
            {
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private bool ContainsOnlyLetters(string input)
        {
            return input.All(c => char.IsLetter(c) || c == ' ');
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ElementsToolsClass.AllFieldsFilled(this))
                {
                    if (PhoneNumberGuestTB.Text.Length != 11)
                    {
                        ShowErrorMessage("Номер телефона должен содержать ровно 11 символов");
                        return;
                    }

                    if (!PhoneNumberGuestTB.Text.All(char.IsDigit))
                    {
                        ShowErrorMessage("Номер телефона должен содержать только цифры");
                        return;
                    }

                    if (!ContainsOnlyLetters(LastNameGuestTB.Text) || !ContainsOnlyLetters(FirstNameGuestTB.Text) || !ContainsOnlyLetters(MiddleNameGuestTB.Text))
                    {
                        ShowErrorMessage("Фамилия, имя и отчество должны содержать только буквы");
                        return;
                    }

                    // Преобразование значений из ComboBox
                    int idRoom = GetSelectedItemId<Room>(RoomCB);
                    int idGender = GetSelectedItemId<Gender>(GenderCB);

                    var context = DBEntities.GetContext();

                    // Поиск существующего гостя по ID
                    var existingGuest = context.Guests.FirstOrDefault(g => g.IdGuest == guest.IdGuest);
                    if (existingGuest != null)
                    {
                        context.SaveChanges();
                        ShowSuccessMessage("Данные гостя обновлены");
                        Close();
                    }
                    else
                    {
                        ShowErrorMessage("Гость не найден");
                    }
                }
                else
                {
                    ShowErrorMessage("Не все поля заполнены!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при редактировании гостя: {ex.Message}");
            }
        }
    }
}
