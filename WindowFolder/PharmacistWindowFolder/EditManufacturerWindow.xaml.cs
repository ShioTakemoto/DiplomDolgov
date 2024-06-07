using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    public partial class EditManufacturerWindow : Window
    {
        private Manufacturer originalManufacturer;
        private Manufacturer editedManufacturer;

        public EditManufacturerWindow(Manufacturer manufacturer)
        {
            InitializeComponent();
            originalManufacturer = manufacturer;
            editedManufacturer = new Manufacturer
            {
                IdManufacturer = manufacturer.IdManufacturer,
                NameManufacturer = manufacturer.NameManufacturer,
                Address = manufacturer.Address,
                PhoneNumberContactPersonManufacturer = manufacturer.PhoneNumberContactPersonManufacturer,
                EmailManufacturer = manufacturer.EmailManufacturer,
                ContactPersonName = manufacturer.ContactPersonName,
                ManufacturerCountry = manufacturer.ManufacturerCountry
            };
            DataContext = editedManufacturer;
            LoadManufacturerCountries();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadManufacturerCountries()
        {
            var context = DBEntities.GetContext();
            var countries = context.ManufacturerCountry.ToList();
            ManufacturerCountryCB.ItemsSource = countries;

            // Устанавливаем выбранный элемент равным стране производителя
            if (originalManufacturer.ManufacturerCountry != null)
            {
                ManufacturerCountryCB.SelectedItem = countries.FirstOrDefault(c => c.IdManufacturerCountry == originalManufacturer.ManufacturerCountry.IdManufacturerCountry);
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog() == true)
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
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
                    ShowErrorMessage("Поле не должно быть пустым!");
                else if (!IsValidName(inputText))
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы и пробелы.");
                else
                    AddNewManufacturerCountry(inputText);
            }
        }

        private void AddNewManufacturerCountry(string inputText)
        {
            var context = DBEntities.GetContext();
            if (context.ManufacturerCountry.Any(mc => mc.NameManufacturerCountry == inputText))
                ShowErrorMessage("Такая страна уже существует!");
            else
            {
                var newManufacturerCountry = new ManufacturerCountry { NameManufacturerCountry = inputText };
                context.ManufacturerCountry.Add(newManufacturerCountry);
                context.SaveChanges();

                ManufacturerCountryCB.ItemsSource = context.ManufacturerCountry.ToList();
                ManufacturerCountryCB.SelectedItem = newManufacturerCountry;
            }
        }

        private bool IsValidName(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsLetter(c) && c != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ElementsToolsClass.AllFieldsFilled(this))
                {
                    ShowErrorMessage("Не все поля заполнены!");
                    return;
                }

                if (PhoneNumberContactPersonManufacturerTB.Text.Length != 11)
                {
                    ShowErrorMessage("Номер телефона должен содержать ровно 11 символов");
                    return;
                }

                if (!ContactPersonNameTB.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    ShowErrorMessage("Имя контактной персоны должно содержать только буквы");
                    return;
                }

                SaveChanges();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Произошла ошибка: {ex.Message}");
            }
        }

        private void SaveChanges()
        {
            var context = DBEntities.GetContext();

            // Присваиваем новые значения из editedManufacturer в originalManufacturer
            originalManufacturer.NameManufacturer = editedManufacturer.NameManufacturer;
            originalManufacturer.Address = editedManufacturer.Address;
            originalManufacturer.PhoneNumberContactPersonManufacturer = editedManufacturer.PhoneNumberContactPersonManufacturer;
            originalManufacturer.EmailManufacturer = editedManufacturer.EmailManufacturer;
            originalManufacturer.ContactPersonName = editedManufacturer.ContactPersonName;
            originalManufacturer.ManufacturerCountry = editedManufacturer.ManufacturerCountry;

            // Применяем изменения к originalManufacturer
            context.Entry(originalManufacturer).State = EntityState.Modified;

            context.SaveChanges();
            ShowMessageBox("Данные успешно сохранены");
            Close();
        }

        private void ShowMessageBox(string message) => new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();

        private void ShowErrorMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();
    }
}