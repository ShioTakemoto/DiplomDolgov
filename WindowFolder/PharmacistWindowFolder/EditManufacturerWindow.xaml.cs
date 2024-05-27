using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    public partial class EditManufacturerWindow : Window
    {
        private Manufacturer manufacturer;

        public EditManufacturerWindow(Manufacturer manufacturer)
        {
            InitializeComponent();
            this.manufacturer = manufacturer;
            DataContext = this.manufacturer;
            LoadManufacturerCountries();
        }

        private void LoadManufacturerCountries()
        {
            ManufacturerCountryCB.ItemsSource = DBEntities.GetContext().ManufacturerCountry.ToList();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog() == true)
                Close();
        }

        private void AddManufacturerCountry(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новую страну производителя");
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                    ShowMessageBox("Поле не должно быть пустым!");
                else if (!IsValidName(inputText))
                    ShowMessageBox("Недопустимые символы! Допускаются только буквы.");
                else
                    AddNewManufacturerCountry(inputText);
            }
        }

        private void AddNewManufacturerCountry(string inputText)
        {
            var context = DBEntities.GetContext();
            if (context.ManufacturerCountry.Any(mc => mc.NameManufacturerCountry == inputText))
                ShowMessageBox("Такая страна уже существует!");
            else
            {
                var newManufacturerCountry = new ManufacturerCountry { NameManufacturerCountry = inputText };
                context.ManufacturerCountry.Add(newManufacturerCountry);
                context.SaveChanges();

                ManufacturerCountryCB.ItemsSource = context.ManufacturerCountry.ToList();
                ManufacturerCountryCB.SelectedItem = newManufacturerCountry;
            }
        }

        private bool IsValidName(string input) => input.All(char.IsLetter);

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ElementsToolsClass.AllFieldsFilled(this))
                {
                    ShowMessageBox("Не все поля заполнены!");
                    return;
                }

                if (!PhoneNumberContactPersonManufacturerTB.Text.All(char.IsDigit))
                {
                    ShowMessageBox("Номер телефона должен содержать только цифры");
                    return;
                }

                if (!ContactPersonNameTB.Text.All(char.IsLetter))
                {
                    ShowMessageBox("Имя контактной персоны должно содержать только буквы");
                    return;
                }

                SaveChanges();
            }
            catch (Exception ex)
            {
                ShowMessageBox($"Произошла ошибка: {ex.Message}");
            }
        }

        private void SaveChanges()
        {
            DBEntities.GetContext().SaveChanges();
            ShowMessageBox("Данные успешно сохранены");
            Close();
        }

        private void ShowMessageBox(string message) => new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();
    }
}
