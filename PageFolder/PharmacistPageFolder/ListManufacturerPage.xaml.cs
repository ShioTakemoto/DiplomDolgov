using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.AdminWindowFolder;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomDolgov.PageFolder.PharmacistPageFolder
{
    /// <summary>
    /// Логика взаимодействия для ListManufacturerPage.xaml
    /// </summary>
    public partial class ListManufacturerPage : Page
    {
        public ListManufacturerPage()
        {
            InitializeComponent();
            LoadManufacturerCountries();
        }

        private void LoadManufacturerCountries()
        {
            // Очищаем текущие элементы в ComboBox
            ManufacturerCountryCB.Items.Clear();
            // Добавляем пустой элемент
            ManufacturerCountryCB.Items.Add(""); // Пустой элемент
            // Загружаем список стран
            foreach (var country in DBEntities.GetContext().ManufacturerCountry.ToList())
            {
                ManufacturerCountryCB.Items.Add(country);
            }
        }

        private void Search()
        {
            string searchText = SearchTB.Text.Trim().ToLower();

            // Проверяем выбран ли пустой элемент
            if (ManufacturerCountryCB.SelectedItem != null && ManufacturerCountryCB.SelectedItem.ToString() == "")
            {
                // Если выбран пустой элемент, не применяем фильтрацию по стране
                ListManufacturerDG.ItemsSource = DBEntities.GetContext().Manufacturer
                                                .Where(m => string.IsNullOrEmpty(searchText) ||
                                                            (m.NameManufacturer.ToLower().Contains(searchText) ||
                                                             m.Address.ToLower().Contains(searchText) ||
                                                             m.PhoneNumberContactPersonManufacturer.ToLower().Contains(searchText) ||
                                                             m.EmailManufacturer.ToLower().Contains(searchText) ||
                                                             m.ContactPersonName.ToLower().Contains(searchText)))
                                                .ToList();
            }
            else
            {
                int countryId = 0;

                if (ManufacturerCountryCB.SelectedItem != null && ManufacturerCountryCB.SelectedItem is ManufacturerCountry selectedCountry)
                {
                    countryId = selectedCountry.IdManufacturerCountry;
                }

                try
                {
                    var context = DBEntities.GetContext();
                    IQueryable<Manufacturer> query = context.Manufacturer.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        query = query.Where(m => m.NameManufacturer.ToLower().Contains(searchText) ||
                                                 m.Address.ToLower().Contains(searchText) ||
                                                 m.PhoneNumberContactPersonManufacturer.ToLower().Contains(searchText) ||
                                                 m.EmailManufacturer.ToLower().Contains(searchText) ||
                                                 m.ContactPersonName.ToLower().Contains(searchText));
                    }

                    if (countryId != 0)
                    {
                        query = query.Where(m => m.IdManufacturerCountry == countryId);
                    }

                    ListManufacturerDG.ItemsSource = query.ToList();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedManufacturer = ListManufacturerDG.SelectedItem as Manufacturer;

                if (selectedManufacturer == null)
                {
                    new MaterialDesignMessageBox("Выберите производителя для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                using (var context = DBEntities.GetContext())
                {
                    var manufacturer = context.Manufacturer.FirstOrDefault(u => u.IdManufacturer == selectedManufacturer.IdManufacturer);

                    if (manufacturer == null)
                    {
                        new MaterialDesignMessageBox("Производитель не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {manufacturer.NameManufacturer}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

                    if (result == true)
                    {
                        context.Manufacturer.Remove(manufacturer);
                        context.SaveChanges();
                        new MaterialDesignMessageBox("Производитель успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                        RefreshDataGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void EditM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedManufacturer = ListManufacturerDG.SelectedItem as Manufacturer;

            if (selectedManufacturer == null)
            {
                new MaterialDesignMessageBox("Выберите производителя для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var manufacturer = DBEntities.GetContext().Manufacturer.FirstOrDefault(u => u.IdManufacturer == selectedManufacturer.IdManufacturer);

            if (manufacturer == null)
            {
                new MaterialDesignMessageBox("Производитель не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            new EditManufacturerWindow(manufacturer).ShowDialog();
        }

        private void RefreshDataGrid()
        {
            ListManufacturerDG.ItemsSource = DBEntities.GetContext().Manufacturer.ToList().OrderBy(u => u.NameManufacturer);
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void ManufacturerCountryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Search();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new AddManufacturerWindow().Show();
            RefreshDataGrid();
        }
    }
}
