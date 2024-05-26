﻿using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using DiplomDolgov.WindowFolder.SharedWindowsFolder;
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
    /// Логика взаимодействия для PharmacistListMedicinePage.xaml
    /// </summary>
    public partial class PharmacistListMedicinePage : Page
    {
        private string searchText = string.Empty;
        private string selectedType = string.Empty;
        private string selectedActiveSubstance = string.Empty;
        public PharmacistListMedicinePage()
        {
            InitializeComponent();
            LoadMedicines();
            LoadMedicineTypes();
            LoadActiveSubstance();
        }

        private void LoadMedicines()
        {
            var medicines = DBEntities.GetContext().Medicine.ToList();
            foreach (var medicine in medicines)
            {
                medicine.StockCount = DBEntities.GetContext().StockUnit
                    .Where(su => su.IdMedicine == medicine.IdMedicine)
                    .Sum(su => (int?)su.Count) ?? 0;
            }
            ListMedicineLB.ItemsSource = medicines;
            FilterMedicines();
        }

        private void LoadMedicineTypes()
        {
            var types = DBEntities.GetContext().TypeMedicine.ToList();

            // Добавляем пустой вариант в начало списка
            types.Insert(0, new TypeMedicine { });

            TypeMedicineCB.ItemsSource = types;
        }

        private void LoadActiveSubstance()
        {
            var activeSubstances = DBEntities.GetContext().ActiveSubstance.ToList();

            // Добавляем пустой вариант в начало списка
            activeSubstances.Insert(0, new ActiveSubstance { });

            ActiveSubstanceCB.ItemsSource = activeSubstances;
        }

        private void FilterMedicines()
        {
            var filteredMedicines = DBEntities.GetContext().Medicine.ToList();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredMedicines = filteredMedicines
                    .Where(m => m.NameMedicine.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(selectedType))
            {
                filteredMedicines = filteredMedicines
                    .Where(m => m.TypeMedicine.NameTypeMedicine == selectedType)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(selectedActiveSubstance))
            {
                filteredMedicines = filteredMedicines
                    .Where(m => m.ActiveSubstance.NameActiveSubstance == selectedActiveSubstance)
                    .ToList();
            }

            foreach (var medicine in filteredMedicines)
            {
                medicine.StockCount = DBEntities.GetContext().StockUnit
                    .Where(su => su.IdMedicine == medicine.IdMedicine)
                    .Sum(su => (int?)su.Count) ?? 0;
            }

            ListMedicineLB.ItemsSource = filteredMedicines;
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedMedicine = ListMedicineLB.SelectedItem as Medicine;

                if (selectedMedicine == null)
                {
                    new MaterialDesignMessageBox("Выберите препарат для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                var medicine = DBEntities.GetContext().Medicine.FirstOrDefault(u => u.NameMedicine == selectedMedicine.NameMedicine);

                if (medicine == null)
                {
                    new MaterialDesignMessageBox("Препарат не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {medicine.NameMedicine}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

                if (result == true)
                {
                    // Удаление связанных записей из Realization
                    var realizations = DBEntities.GetContext().Realization.Where(r => r.IdMedicine == medicine.IdMedicine).ToList();
                    DBEntities.GetContext().Realization.RemoveRange(realizations);

                    // Удаление связанных записей из Orders
                    var orders = DBEntities.GetContext().Orders.Where(o => o.IdMedicine == medicine.IdMedicine).ToList();
                    DBEntities.GetContext().Orders.RemoveRange(orders);

                    // Удаление строк из StockUnit
                    var stockUnits = DBEntities.GetContext().StockUnit.Where(su => su.IdMedicine == medicine.IdMedicine).ToList();
                    DBEntities.GetContext().StockUnit.RemoveRange(stockUnits);

                    // Удаление лекарства
                    DBEntities.GetContext().Medicine.Remove(medicine);
                    DBEntities.GetContext().SaveChanges();

                    new MaterialDesignMessageBox($"{medicine.NameMedicine} успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();

                    // Обновление ListBox
                    LoadMedicines();
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void EditM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedMedicine = ListMedicineLB.SelectedItem as Medicine;

            if (selectedMedicine == null)
            {
                new MaterialDesignMessageBox("Выберите медикамент для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var medicine = DBEntities.GetContext().Medicine.FirstOrDefault(u => u.IdMedicine == selectedMedicine.IdMedicine);

            if (medicine == null)
            {
                new MaterialDesignMessageBox("Медикамент не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            new EditMedicineWindow(medicine).ShowDialog();
            LoadMedicines();
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = SearchTB.Text;
            FilterMedicines();
        }

        private void ListMedicineLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedMedicine = ListMedicineLB.SelectedItem as Medicine;
            if (selectedMedicine != null)
            {
                var detailsWindow = new MedicineDetailsWindow(selectedMedicine);
                detailsWindow.Show();
            }
        }

        private void TypeMedicineCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedType = (TypeMedicineCB.SelectedItem as TypeMedicine)?.NameTypeMedicine ?? string.Empty;
            FilterMedicines();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new AddMedicineWindow().Show();
            LoadMedicines();
        }

        private void ActiveSubstanceCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedActiveSubstance = (ActiveSubstanceCB.SelectedItem as ActiveSubstance)?.NameActiveSubstance ?? string.Empty;
            FilterMedicines();
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = FindParent<ScrollViewer>((DependencyObject)sender);
            if (scrollViewer != null)
            {
                if (e.Delta > 0)
                    scrollViewer.LineUp();
                else
                    scrollViewer.LineDown();
                e.Handled = true;
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}