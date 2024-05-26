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
    /// Логика взаимодействия для AddManufacturerWindow.xaml
    /// </summary>
    public partial class AddManufacturerWindow : Window
    {
        public AddManufacturerWindow()
        {
            InitializeComponent();
            ManufacturerCountryCB.ItemsSource = DBEntities.GetContext().ManufacturerCountry.ToList();
        }

        private void AddManufacturerCountry(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новую страну производителя");
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    new MaterialDesignMessageBox("Поле не должно быть пустым!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (!IsValidName(inputText))
                {
                    new MaterialDesignMessageBox("Недопустимые символы! Допускаются только буквы.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else
                {
                    var context = DBEntities.GetContext();
                    if (context.ManufacturerCountry.Any(mc => mc.NameManufacturerCountry == inputText))
                    {
                        new MaterialDesignMessageBox("Такая страна уже существует!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                    else
                    {
                        var newManufacturerCountry = new ManufacturerCountry { NameManufacturerCountry = inputText };
                        context.ManufacturerCountry.Add(newManufacturerCountry);
                        context.SaveChanges();

                        ManufacturerCountryCB.ItemsSource = context.ManufacturerCountry.ToList();
                        ManufacturerCountryCB.SelectedItem = newManufacturerCountry;
                    }
                }
            }
        }
        private bool IsValidName(string input)
        {
            return input.All(char.IsLetter);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                    // Проверка телефонного номера на наличие только цифр
                    if (!PhoneNumberContactPersonManufacturerTB.Text.All(char.IsDigit))
                    {
                        new MaterialDesignMessageBox("Номер телефона должен содержать только цифры", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    // Проверка имени контактной персоны на наличие только букв
                    if (!ContactPersonNameTB.Text.All(char.IsLetter))
                    {
                        new MaterialDesignMessageBox("Имя контактной персоны должно содержать только буквы", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    // Преобразование значений из комбобоксов
                    int idManufacturerCountry = Convert.ToInt32(ManufacturerCountryCB.SelectedValue);

                    var context = DBEntities.GetContext();

                    var existingManufacturer = context.Manufacturer
                        .FirstOrDefault(m =>
                            m.NameManufacturer == NameManufacturerTB.Text &&
                            m.Address == AddressTB.Text &&
                            m.PhoneNumberContactPersonManufacturer == PhoneNumberContactPersonManufacturerTB.Text &&
                            m.EmailManufacturer == EmailManufacturerTB.Text &&
                            m.ContactPersonName == ContactPersonNameTB.Text &&
                            m.IdManufacturerCountry == idManufacturerCountry);

                    if (existingManufacturer != null)
                    {
                        new MaterialDesignMessageBox("Такой производитель уже существует", MessageType.Warning, MessageButtons.Ok).ShowDialog();
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
                        new MaterialDesignMessageBox("Производитель добавлен", MessageType.Success, MessageButtons.Ok).ShowDialog();
                        ElementsToolsClass.ClearAllControls(this);
                    }
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
    }
}
