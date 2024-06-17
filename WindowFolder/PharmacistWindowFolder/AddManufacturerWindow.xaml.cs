using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AddManufacturerWindow.xaml
    /// </summary>
    public partial class AddManufacturerWindow : Window
    {
        public event EventHandler AddedManufacturer;
        public AddManufacturerWindow()
        {
            InitializeComponent();
            LoadManufacturerCountries();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadManufacturerCountries()
        {
            ManufacturerCountryCB.ItemsSource = DBEntities.GetContext().ManufacturerCountry.ToList();
        }

        // Обработчик добавления новой страны производителя
        private void AddManufacturerCountry(object sender, RoutedEventArgs e)
        {
            // Создаем окно для ввода новой страны производителя
            var inputWindow = new InputDialogWindow("Введите новую страну производителя")
            {
                Topmost = true // Устанавливаем окно на передний план
            };

            // Отображаем окно и ожидаем ввода пользователя
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;

                // Проверяем, что поле не пустое
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                // Проверяем, что введены только допустимые символы
                else if (!IsValidName(inputText))
                {
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы.");
                }
                else
                {
                    // Вызываем метод для добавления новой страны производителя
                    AddNewManufacturerCountry(inputText);
                }
            }
        }

        // Метод для добавления новой страны производителя в базу данных
        private void AddNewManufacturerCountry(string inputText)
        {
            // Получаем контекст базы данных
            var context = DBEntities.GetContext();

            // Проверяем, существует ли уже страна с таким же именем
            if (context.ManufacturerCountry.Any(mc => mc.NameManufacturerCountry == inputText))
            {
                // Выводим сообщение об ошибке, если страна уже существует
                ShowErrorMessage("Такая страна уже существует!");
            }
            else
            {
                // Создаем новый объект страны производителя
                var newManufacturerCountry = new ManufacturerCountry { NameManufacturerCountry = inputText };

                // Добавляем новую страну в базу данных и сохраняем изменения
                context.ManufacturerCountry.Add(newManufacturerCountry);
                context.SaveChanges();

                // Перезагружаем список стран производителей
                LoadManufacturerCountries();

                // Устанавливаем новую страну производителя как выбранную в ComboBox
                ManufacturerCountryCB.SelectedItem = newManufacturerCountry;
            }
        }

        private bool IsValidName(string inputText)
        {
            // Разрешаем буквы и пробелы
            return !string.IsNullOrWhiteSpace(inputText) && inputText.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowConfirmationMessage("Вы уверены что хотите выйти?"))
            {
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void AddButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ElementsToolsClass.AllFieldsFilled(this))
                {
                    if (PhoneNumberContactPersonManufacturerTB.Text.Length != 11)
                    {
                        ShowErrorMessage("Номер телефона должен содержать ровно 11 символов");
                        return;
                    }

                    if (!PhoneNumberContactPersonManufacturerTB.Text.All(char.IsDigit))
                    {
                        ShowErrorMessage("Номер телефона должен содержать только цифры");
                        return;
                    }

                    if (!ContactPersonNameTB.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    {
                        ShowErrorMessage("Имя контактной персоны должно содержать только буквы");
                        return;
                    }

                    // Преобразование значений из комбобоксов
                    int idManufacturerCountry = GetSelectedItemId<ManufacturerCountry>(ManufacturerCountryCB);

                    var context = DBEntities.GetContext();

                    var existingManufacturer = context.Manufacturer.FirstOrDefault(m =>
                        m.NameManufacturer == NameManufacturerTB.Text &&
                        m.Address == AddressTB.Text &&
                        m.PhoneNumberContactPersonManufacturer == PhoneNumberContactPersonManufacturerTB.Text &&
                        m.EmailManufacturer == EmailManufacturerTB.Text &&
                        m.ContactPersonName == ContactPersonNameTB.Text &&
                        m.IdManufacturerCountry == idManufacturerCountry);

                    if (existingManufacturer != null)
                    {
                        ShowWarningMessage("Такой производитель уже существует");
                    }
                    else
                    {
                        var newManufacturer = new Manufacturer
                        {
                            NameManufacturer = NameManufacturerTB.Text,
                            Address = AddressTB.Text,
                            PhoneNumberContactPersonManufacturer = PhoneNumberContactPersonManufacturerTB.Text,
                            EmailManufacturer = EmailManufacturerTB.Text,
                            ContactPersonName = ContactPersonNameTB.Text,
                            IdManufacturerCountry = idManufacturerCountry
                        };

                        context.Manufacturer.Add(newManufacturer);
                        context.SaveChanges();
                        ShowSuccessMessage("Производитель добавлен");
                        ElementsToolsClass.ClearAllControls(this);
                        AddedManufacturer?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    ShowErrorMessage("Не все поля заполнены!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Произошла ошибка: {ex.Message}");
            }
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
            // Получаем выбранный элемент из комбо-бокса
            var selectedItem = comboBox.SelectedItem;

            // Проверяем, что выбранный элемент не равен null
            if (selectedItem != null)
            {
                // Получаем тип выбранного элемента и получаем его свойство "Id{тип T}"
                // Приводим значение свойства к типу int и возвращаем его
                return (int)selectedItem.GetType().GetProperty($"Id{typeof(T).Name}").GetValue(selectedItem);
            }

            // Если выбранный элемент равен null, возвращаем -1
            return -1;
        }
    }
}
