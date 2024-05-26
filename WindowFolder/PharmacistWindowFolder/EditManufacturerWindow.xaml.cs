using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для EditManufacturerWindow.xaml
    /// </summary>
    public partial class EditManufacturerWindow : Window
    {
        private Manufacturer manufacturer;

        public EditManufacturerWindow(Manufacturer manufacturer)
        {
            InitializeComponent();
            this.manufacturer = manufacturer;
            DataContext = this.manufacturer;
            ManufacturerCountryCB.ItemsSource = DBEntities.GetContext().ManufacturerCountry.ToList();
        }

        

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                        DBEntities.GetContext().ManufacturerCountry.Add(newManufacturerCountry);
                        DBEntities.GetContext().SaveChanges();

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

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ElementsToolsClass.AllFieldsFilled(this))
                {
                    // Проверка корректности номера телефона
                    if (!PhoneNumberContactPersonManufacturerTB.Text.All(char.IsDigit))
                    {
                        new MaterialDesignMessageBox("Номер телефона должен содержать только цифры", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    // Проверка корректности имени контактного лица
                    if (!ContactPersonNameTB.Text.All(char.IsLetter))
                    {
                        new MaterialDesignMessageBox("Имя контактной персоны должно содержать только буквы", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    var manufacturerToUpdate = DBEntities.GetContext().Manufacturer.FirstOrDefault(u => u.IdManufacturer == manufacturer.IdManufacturer);
                    // Преобразование значения из комбобокса
                    int idManufacturerCountry = Convert.ToInt32(ManufacturerCountryCB.SelectedValue);
                    if (manufacturerToUpdate != null)
                    {
                        // Сохранение данных
                        manufacturerToUpdate.NameManufacturer = NameManufacturerTB.Text;
                        manufacturerToUpdate.Address = AddressTB.Text;
                        manufacturerToUpdate.PhoneNumberContactPersonManufacturer = PhoneNumberContactPersonManufacturerTB.Text;
                        manufacturerToUpdate.EmailManufacturer = EmailManufacturerTB.Text;
                        manufacturerToUpdate.ContactPersonName = ContactPersonNameTB.Text;
                        manufacturerToUpdate.IdManufacturerCountry = idManufacturerCountry;

                        DBEntities.GetContext().SaveChanges();

                        // Обновление ComboBox после сохранения изменений
                        ManufacturerCountryCB.ItemsSource = DBEntities.GetContext().ManufacturerCountry.ToList();

                        new MaterialDesignMessageBox("Данные успешно сохранены", MessageType.Success, MessageButtons.Ok).ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        new MaterialDesignMessageBox("Производитель не найден", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                }
                else
                {
                    new MaterialDesignMessageBox("Не все поля заполнены!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"Произошла ошибка: {ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }
    }
}