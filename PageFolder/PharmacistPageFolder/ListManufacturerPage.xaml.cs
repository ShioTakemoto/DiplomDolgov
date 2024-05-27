using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiplomDolgov.PageFolder.PharmacistPageFolder
{
    public partial class ListManufacturerPage : Page
    {
        public ListManufacturerPage()
        {
            InitializeComponent();
            LoadManufacturerCountries();
            Search();
        }

        private void LoadManufacturerCountries()
        {
            var countries = DBEntities.GetContext().ManufacturerCountry.ToList();
            countries.Insert(0, new ManufacturerCountry { IdManufacturerCountry = 0, NameManufacturerCountry = "" }); // Добавляем пустой элемент
            ManufacturerCountryCB.ItemsSource = countries;
        }


        private void Search()
        {
            var context = DBEntities.GetContext();
            var searchText = SearchTB.Text.Trim().ToLower();
            var selectedCountryName = ManufacturerCountryCB.SelectedItem?.ToString();
            var query = context.Manufacturer.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(m =>
                    m.NameManufacturer.ToLower().Contains(searchText) ||
                    m.Address.ToLower().Contains(searchText) ||
                    m.PhoneNumberContactPersonManufacturer.ToLower().Contains(searchText) ||
                    m.EmailManufacturer.ToLower().Contains(searchText) ||
                    m.ContactPersonName.ToLower().Contains(searchText)
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedCountryName))
            {
                query = query.Where(m =>
                    m.ManufacturerCountry.NameManufacturerCountry.Equals(selectedCountryName)
                );
            }

            try
            {
                ListManufacturerDG.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedManufacturer = ListManufacturerDG.SelectedItem as Manufacturer;

            if (selectedManufacturer == null)
            {
                new MaterialDesignMessageBox("Выберите производителя для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {selectedManufacturer.NameManufacturer}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    var context = DBEntities.GetContext();
                    context.Manufacturer.Remove(selectedManufacturer);
                    context.SaveChanges();
                    new MaterialDesignMessageBox("Производитель успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    RefreshDataGrid();
                    ListManufacturerDG.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
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

            new EditManufacturerWindow(selectedManufacturer).ShowDialog();
            RefreshDataGrid();
            ListManufacturerDG.Items.Refresh();
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
            ListManufacturerDG.Items.Refresh();
        }
    }
}
