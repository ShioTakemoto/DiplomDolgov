using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomDolgov.PageFolder.PharmacistPageFolder
{
    /// <summary>
    /// Логика взаимодействия для AnotherTablesPharmacistPage.xaml
    /// </summary>
    public partial class AnotherTablesPharmacistPage : Page
    {
        public AnotherTablesPharmacistPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var context = DBEntities.GetContext();

            ListTypeMedicineDG.ItemsSource = context.TypeMedicine.ToList();
            ListActiveSubstanceDG.ItemsSource = context.ActiveSubstance.ToList();
            ListReleaseFormDG.ItemsSource = context.ReleaseForm.ToList();
            ListBestBeforeDateDG.ItemsSource = context.BestBeforeDate.ToList();
            ListManufacturerCountryDG.ItemsSource = context.ManufacturerCountry.ToList();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTB.Text.ToLower();

            var typeMedicineView = (CollectionView)CollectionViewSource.GetDefaultView(ListTypeMedicineDG.ItemsSource);
            typeMedicineView.Filter = item =>
            {
                var typeMedicine = item as TypeMedicine;
                return typeMedicine.NameTypeMedicine.ToLower().Contains(searchText);
            };

            var activeSubstanceView = (CollectionView)CollectionViewSource.GetDefaultView(ListActiveSubstanceDG.ItemsSource);
            activeSubstanceView.Filter = item =>
            {
                var activeSubstance = item as ActiveSubstance;
                return activeSubstance.NameActiveSubstance.ToLower().Contains(searchText);
            };

            var releaseFormView = (CollectionView)CollectionViewSource.GetDefaultView(ListReleaseFormDG.ItemsSource);
            releaseFormView.Filter = item =>
            {
                var releaseForm = item as ReleaseForm;
                return releaseForm.NameReleaseForm.ToLower().Contains(searchText);
            };

            var bestBeforeDateView = (CollectionView)CollectionViewSource.GetDefaultView(ListBestBeforeDateDG.ItemsSource);
            bestBeforeDateView.Filter = item =>
            {
                var bestBeforeDate = item as BestBeforeDate;
                return bestBeforeDate.NameBestBeforeDate.ToLower().Contains(searchText);
            };

            var manufacturerCountryView = (CollectionView)CollectionViewSource.GetDefaultView(ListManufacturerCountryDG.ItemsSource);
            manufacturerCountryView.Filter = item =>
            {
                var manufacturerCountry = item as ManufacturerCountry;
                return manufacturerCountry.NameManufacturerCountry.ToLower().Contains(searchText);
            };
        }

        private void DeleteTypeMedicine_Click(object sender, RoutedEventArgs e)
        {
            var selectedTypeMedicine = ListTypeMedicineDG.SelectedItem as TypeMedicine;

            if (selectedTypeMedicine == null)
            {
                new MaterialDesignMessageBox("Выберите тип медикамента для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {selectedTypeMedicine.NameTypeMedicine}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    DBEntities.GetContext().TypeMedicine.Remove(selectedTypeMedicine);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Тип медикамента успешно удален", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadData();
                    ListTypeMedicineDG.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
        }

        private void DeleteActiveSubstance_Click(object sender, RoutedEventArgs e)
        {
            var selectedActiveSubstance = ListActiveSubstanceDG.SelectedItem as ActiveSubstance;

            if (selectedActiveSubstance == null)
            {
                new MaterialDesignMessageBox("Выберите активное вещество для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {selectedActiveSubstance.NameActiveSubstance}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    DBEntities.GetContext().ActiveSubstance.Remove(selectedActiveSubstance);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Активное вещество успешно удалено", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadData();
                    ListActiveSubstanceDG.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
        }

        private void DeleteReleaseForm_Click(object sender, RoutedEventArgs e)
        {
            var selectedReleaseForm = ListReleaseFormDG.SelectedItem as ReleaseForm;

            if (selectedReleaseForm == null)
            {
                new MaterialDesignMessageBox("Выберите форму выпуска для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {selectedReleaseForm.NameReleaseForm}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    DBEntities.GetContext().ReleaseForm.Remove(selectedReleaseForm);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Форма выпуска успешно удалена", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadData();
                    ListReleaseFormDG.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
        }

        private void DeleteBestBeforeDate_Click(object sender, RoutedEventArgs e)
        {
            var selectedBestBeforeDate = ListBestBeforeDateDG.SelectedItem as BestBeforeDate;

            if (selectedBestBeforeDate == null)
            {
                new MaterialDesignMessageBox("Выберите срок годности для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {selectedBestBeforeDate.NameBestBeforeDate}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    DBEntities.GetContext().BestBeforeDate.Remove(selectedBestBeforeDate);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Срок годности успешно удален", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadData();
                    ListBestBeforeDateDG.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
        }

        private void DeleteManufacturerCountry_Click(object sender, RoutedEventArgs e)
        {
            var selectedManufacturerCountry = ListManufacturerCountryDG.SelectedItem as ManufacturerCountry;

            if (selectedManufacturerCountry == null)
            {
                new MaterialDesignMessageBox("Выберите страну производителя для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {selectedManufacturerCountry.NameManufacturerCountry}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    DBEntities.GetContext().ManufacturerCountry.Remove(selectedManufacturerCountry);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Страна производителя успешно удалена", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadData();
                    ListManufacturerCountryDG.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
        }

        private void EditTypeMedicine_Click(object sender, RoutedEventArgs e)
        {
            var selectedTypeMedicine = ListTypeMedicineDG.SelectedItem as TypeMedicine;

            if (selectedTypeMedicine == null)
            {
                new MaterialDesignMessageBox("Выберите тип медикамента для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editWindow = new LittleTablesEditWindow(selectedTypeMedicine, "Тип медикамента");

            var pharmacistWindow = Window.GetWindow(this) as PharmacistWindow;
            if (pharmacistWindow != null)
            {
                // Показываем затемняющий слой
                pharmacistWindow.ShowOverlay1();
                editWindow.Closed += (s, args) => pharmacistWindow.HideOverlay1();
            }

            editWindow.ShowDialog();
            LoadData();
            ListTypeMedicineDG.Items.Refresh();
        }

        private void EditActiveSubstance_Click(object sender, RoutedEventArgs e)
        {
            var selectedActiveSubstance = ListActiveSubstanceDG.SelectedItem as ActiveSubstance;

            if (selectedActiveSubstance == null)
            {
                new MaterialDesignMessageBox("Выберите активное вещество для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editWindow = new LittleTablesEditWindow(selectedActiveSubstance, "Активное вещество");

            var pharmacistWindow = Window.GetWindow(this) as PharmacistWindow;
            if (pharmacistWindow != null)
            {
                // Показываем затемняющий слой
                pharmacistWindow.ShowOverlay1();
                editWindow.Closed += (s, args) => pharmacistWindow.HideOverlay1();
            }

            editWindow.ShowDialog();
            LoadData();
            ListActiveSubstanceDG.Items.Refresh();
        }

        private void EditReleaseForm_Click(object sender, RoutedEventArgs e)
        {
            var selectedReleaseForm = ListReleaseFormDG.SelectedItem as ReleaseForm;

            if (selectedReleaseForm == null)
            {
                new MaterialDesignMessageBox("Выберите форму выпуска для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editWindow = new LittleTablesEditWindow(selectedReleaseForm, "Форма выпуска");

            var pharmacistWindow = Window.GetWindow(this) as PharmacistWindow;
            if (pharmacistWindow != null)
            {
                // Показываем затемняющий слой
                pharmacistWindow.ShowOverlay1();
                editWindow.Closed += (s, args) => pharmacistWindow.HideOverlay1();
            }

            editWindow.ShowDialog();
            LoadData();
            ListReleaseFormDG.Items.Refresh();
        }

        private void EditBestBeforeDate_Click(object sender, RoutedEventArgs e)
        {
            var selectedBestBeforeDate = ListBestBeforeDateDG.SelectedItem as BestBeforeDate;

            if (selectedBestBeforeDate == null)
            {
                new MaterialDesignMessageBox("Выберите срок годности для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editWindow = new LittleTablesEditWindow(selectedBestBeforeDate, "Срок годности");

            var pharmacistWindow = Window.GetWindow(this) as PharmacistWindow;
            if (pharmacistWindow != null)
            {
                // Показываем затемняющий слой
                pharmacistWindow.ShowOverlay1();
                editWindow.Closed += (s, args) => pharmacistWindow.HideOverlay1();
            }

            editWindow.ShowDialog();
            LoadData();
            ListBestBeforeDateDG.Items.Refresh();
        }

        private void EditManufacturerCountry_Click(object sender, RoutedEventArgs e)
        {
            var selectedManufacturerCountry = ListManufacturerCountryDG.SelectedItem as ManufacturerCountry;

            if (selectedManufacturerCountry == null)
            {
                new MaterialDesignMessageBox("Выберите страну производителя для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editWindow = new LittleTablesEditWindow(selectedManufacturerCountry, "Страна производителя");

            var pharmacistWindow = Window.GetWindow(this) as PharmacistWindow;
            if (pharmacistWindow != null)
            {
                // Показываем затемняющий слой
                pharmacistWindow.ShowOverlay1();
                editWindow.Closed += (s, args) => pharmacistWindow.HideOverlay1();
            }

            editWindow.ShowDialog();
            LoadData();
            ListManufacturerCountryDG.Items.Refresh();
        }
    }
}
