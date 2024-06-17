using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using DiplomDolgov.WindowFolder.SharedWindowsFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DiplomDolgov.PageFolder.MainMedicineWorkerPageFolder
{
    /// <summary>
    /// Логика взаимодействия для MainMedicineWorkerListMedicine.xaml
    /// </summary>
    public partial class MainMedicineWorkerListMedicine : Page
    {
        private string searchText = string.Empty;
        private string selectedType = string.Empty;
        private string selectedActiveSubstance = string.Empty;

        public MainMedicineWorkerListMedicine()
        {
            InitializeComponent();
            LoadMedicines();
            LoadMedicineTypes();
            LoadActiveSubstance();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["PageFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadMedicines()
        {
            var medicines = DBEntities.GetContext().Medicine.ToList();
            UpdateMedicinesList(medicines);
        }

        private void LoadMedicineTypes()
        {
            var types = DBEntities.GetContext().TypeMedicine.ToList();
            types.Insert(0, new TypeMedicine { }); // Добавляем пустой вариант в начало списка
            TypeMedicineCB.ItemsSource = types;
        }

        private void LoadActiveSubstance()
        {
            var activeSubstances = DBEntities.GetContext().ActiveSubstance.ToList();
            activeSubstances.Insert(0, new ActiveSubstance { }); // Добавляем пустой вариант в начало списка
            ActiveSubstanceCB.ItemsSource = activeSubstances;
        }

        private void FilterMedicines()
        {
            var filteredMedicines = DBEntities.GetContext().Medicine.ToList();

            if (!string.IsNullOrWhiteSpace(searchText))
                filteredMedicines = filteredMedicines.Where(m => m.NameMedicine.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            if (!string.IsNullOrWhiteSpace(selectedType))
                filteredMedicines = filteredMedicines.Where(m => m.TypeMedicine.NameTypeMedicine == selectedType).ToList();

            if (!string.IsNullOrWhiteSpace(selectedActiveSubstance))
                filteredMedicines = filteredMedicines.Where(m => m.ActiveSubstance.NameActiveSubstance == selectedActiveSubstance).ToList();

            UpdateMedicinesList(filteredMedicines);
        }

        private void UpdateMedicinesList(IEnumerable<Medicine> medicines)
        {
            foreach (var medicine in medicines)
            {
                medicine.StockCount = DBEntities.GetContext().StockUnit.Where(su => su.IdMedicine == medicine.IdMedicine).Sum(su => (int?)su.Count) ?? 0;
            }
            ListMedicineLB.ItemsSource = medicines;
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
                detailsWindow.Topmost = true; // Установка окна поверх всех окон
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
            LoadMedicines();
            new AddMedicineWindow().Show();
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
                {
                    scrollViewer.LineUp();
                }
                else
                {
                    scrollViewer.LineDown();
                }
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
