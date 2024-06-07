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

        private void AddManufacturerCountry(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новую страну производителя")
            {
                Topmost = true // Устанавливаем окно на передний план
            };
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                else if (!IsValidName(inputText))
                {
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы.");
                }
                else
                {
                    AddNewManufacturerCountry(inputText);
                }
            }
        }

        private void AddNewManufacturerCountry(string inputText)
        {
            var context = DBEntities.GetContext();
            if (context.ManufacturerCountry.Any(mc => mc.NameManufacturerCountry == inputText))
            {
                ShowErrorMessage("Такая страна уже существует!");
            }
            else
            {
                var newManufacturerCountry = new ManufacturerCountry { NameManufacturerCountry = inputText };
                context.ManufacturerCountry.Add(newManufacturerCountry);
                context.SaveChanges();

                LoadManufacturerCountries();
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
            var selectedItem = comboBox.SelectedItem;
            if (selectedItem != null)
            {
                return (int)selectedItem.GetType().GetProperty($"Id{typeof(T).Name}").GetValue(selectedItem);
            }
            return -1;
        }
    }
}
