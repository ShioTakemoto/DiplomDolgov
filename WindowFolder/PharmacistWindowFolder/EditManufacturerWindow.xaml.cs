using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
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

        private void SaveManufacturerButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(NameManufacturerTB.Text))
                {
                    new MaterialDesignMessageBox("Введите название производителя", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (string.IsNullOrEmpty(AddressTB.Text))
                {
                    new MaterialDesignMessageBox("Введите адрес", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (string.IsNullOrEmpty(PhoneNumberContactPersonManufacturerTB.Text))
                {
                    new MaterialDesignMessageBox("Введите номер телефона", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (string.IsNullOrEmpty(EmailManufacturerTB.Text))
                {
                    new MaterialDesignMessageBox("Введите почту", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (string.IsNullOrEmpty(ContactPersonNameTB.Text))
                {
                    new MaterialDesignMessageBox("Введите имя контактного лица", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else if (ManufacturerCountryCB.SelectedIndex == -1)
                {
                    new MaterialDesignMessageBox("Вы не выбрали страну производителя", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
                else
                {
                    var manufacturerToUpdate = DBEntities.GetContext().Manufacturer.FirstOrDefault(m => m.IdManufacturer == manufacturer.IdManufacturer);

                    if (manufacturerToUpdate != null)
                    {
                        manufacturerToUpdate.NameManufacturer = NameManufacturerTB.Text;
                        manufacturerToUpdate.Address = AddressTB.Text;
                        manufacturerToUpdate.PhoneNumberContactPersonManufacturer = PhoneNumberContactPersonManufacturerTB.Text;
                        manufacturerToUpdate.EmailManufacturer = EmailManufacturerTB.Text;
                        manufacturerToUpdate.ContactPersonName = ContactPersonNameTB.Text;
                        manufacturerToUpdate.IdManufacturerCountry = (int)ManufacturerCountryCB.SelectedValue;

                        DBEntities.GetContext().SaveChanges();
                        new MaterialDesignMessageBox("Данные успешно сохранены", MessageType.Success, MessageButtons.Ok).ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        new MaterialDesignMessageBox("Производитель не найден", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
        private bool IsValidName(string input)
        {
            return input.All(char.IsLetter);
        }
    }
}
