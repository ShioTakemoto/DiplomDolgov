using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using DiplomDolgov.WindowFolder.SharedWindowsFolder;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DiplomDolgov.PageFolder.PharmacistPageFolder
{
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["PageFadeIn"];
            fadeInAnimation.Begin(this);
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
            var allTypes = DBEntities.GetContext().TypeMedicine.ToList();
            allTypes.Insert(0, new TypeMedicine { NameTypeMedicine = "Все" });
            TypeMedicineCB.ItemsSource = allTypes;
            TypeMedicineCB.SelectedIndex = 0;
        }

        private void LoadActiveSubstance()
        {
            var allSubstances = DBEntities.GetContext().ActiveSubstance.ToList();
            allSubstances.Insert(0, new ActiveSubstance { NameActiveSubstance = "Все" });
            ActiveSubstanceCB.ItemsSource = allSubstances;
            ActiveSubstanceCB.SelectedIndex = 0;
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
                    new MaterialDesignMessageBox("Выберите медикамент для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }
                var medicine = DBEntities.GetContext().Medicine.FirstOrDefault(u => u.IdMedicine == selectedMedicine.IdMedicine);

                if (medicine == null)
                {
                    new MaterialDesignMessageBox("Медикамент не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {medicine.NameMedicine}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

                if (result == true)
                {
                    DBEntities.GetContext().Medicine.Remove(medicine);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox($"Медикамент успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadMedicines();
                    ListMedicineLB.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"Ошибка: {ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
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

            var editMedicineWindow = new EditMedicineWindow(medicine);

            // Получаем ссылку на родительское окно
            var parentWindow = Window.GetWindow(this) as PharmacistWindow;
            if (parentWindow != null)
            {
                // Показываем затемняющий слой
                parentWindow.ShowOverlay1();
                editMedicineWindow.Closed += (s, args) => parentWindow.HideOverlay1();
            }

            editMedicineWindow.ShowDialog();
            LoadMedicines();
            ListMedicineLB.Items.Refresh();
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

        private void AddMedicineWindow_AddedMedicine(object sender, EventArgs e)
        {
            // Обновляем DataGrid при получении события об успешном добавлении медикамента
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

        // Метод для поиска родительского объекта типа T в визуальном дереве
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // Получаем родительский объект для данного объекта зависимости
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // Проверяем, есть ли у child родительский объект. Если нет, возвращаем null, потому что больше нечего искать.
            if (parentObject == null)
                return null;

            // Пытаемся преобразовать parentObject к типу T
            T parent = parentObject as T;

            // Проверяем, удалось ли преобразовать parentObject в объект типа T
            if (parent != null)
                return parent; // Если преобразование удалось, возвращаем найденный родительский объект
            else
                return FindParent<T>(parentObject); // Если преобразование не удалось, продолжаем поиск родительских объектов рекурсивно
        }

        private void AddMedicineBtn_Click(object sender, RoutedEventArgs e)
        {
            var addMedicineWindow = new AddMedicineWindow();

            var pharmacistWindow = Window.GetWindow(this) as PharmacistWindow;
            if (pharmacistWindow != null)
            {
                // Показываем затемняющий слой
                pharmacistWindow.ShowOverlay1();
                addMedicineWindow.Closed += (s, args) => pharmacistWindow.HideOverlay1();
            }

            // Подписываемся на событие AddedMedicine, которое будет вызываться после успешного добавления медикамента
            addMedicineWindow.AddedMedicine += AddMedicineWindow_AddedMedicine;

            addMedicineWindow.Topmost = true; // Устанавливаем на передний план
            addMedicineWindow.ShowDialog();
            addMedicineWindow.Activate(); // Активируем окно
        }
    }
}
