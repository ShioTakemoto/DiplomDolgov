using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    public partial class EditMedicineWindow : Window
    {
        private string selectedFileName = "";
        private Medicine medicine;
        private DBEntities context;

        public EditMedicineWindow(Medicine medicine)
        {
            InitializeComponent();
            this.medicine = medicine;
            DataContext = this.medicine;
            context = DBEntities.GetContext(); // Создание контекста данных
            LoadData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadData()
        {
            var context = DBEntities.GetContext();
            ActiveSubstanceCB.ItemsSource = context.ActiveSubstance.ToList();
            ReleaseFormCB.ItemsSource = context.ReleaseForm.ToList();
            BestBeforeDateCB.ItemsSource = context.BestBeforeDate.ToList();
            PrescriptionDrugStatusCB.ItemsSource = context.PrescriptionDrugStatus.ToList();
            TypeMedicineCB.ItemsSource = context.TypeMedicine.ToList();
            ManufacturerCB.ItemsSource = context.Manufacturer.ToList();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
        }

        private void AddPhoto(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                op.InitialDirectory = "";
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png";

                if (op.ShowDialog() == true)
                {
                    selectedFileName = op.FileName;
                    medicine.MedicinePhoto = ImageClass.ConvertImageToByteArray(selectedFileName);
                    ImPhoto.Source = ImageClass.ConvertByteArrayToImage(medicine.MedicinePhoto);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void AddTypeMedicine(object sender, RoutedEventArgs e) => AddNewItem<TypeMedicine>("Введите новый тип медикамента", TypeMedicineCB, tm => tm.NameTypeMedicine);

        private void AddActiveSubstance(object sender, RoutedEventArgs e) => AddNewItem<ActiveSubstance>("Введите новое активное вещество", ActiveSubstanceCB, acs => acs.NameActiveSubstance);

        private void AddReleaseForm(object sender, RoutedEventArgs e) => AddNewItem<ReleaseForm>("Введите новую форму выпуска", ReleaseFormCB, rf => rf.NameReleaseForm);

        private void AddBestBeforeDate(object sender, RoutedEventArgs e) => AddNewItem<BestBeforeDate>("Введите новый срок годности", BestBeforeDateCB, bb => bb.NameBestBeforeDate);

        // Метод для добавления нового элемента типа T в базу данных и ComboBox
        private void AddNewItem<T>(string message, ComboBox comboBox, Func<T, string> selector) where T : class, new()
        {
            // Показываем диалоговое окно для ввода нового элемента
            var inputText = ShowInputDialog(message);

            // Проверяем, что введенный текст не пустой
            if (string.IsNullOrEmpty(inputText))
            {
                ShowErrorMessage("Поле не должно быть пустым!");
                return;
            }

            // Проверяем валидность введенного имени
            if (!IsValidName(inputText))
            {
                ShowErrorMessage("Недопустимые символы! Допускаются только буквы и пробелы.");
                return;
            }

            // Получаем контекст базы данных
            var context = DBEntities.GetContext();
            var dbSet = context.Set<T>();
            var propertyName = $"Name{typeof(T).Name}";

            // Получаем значение свойства за пределами запроса LINQ
            var propertyValue = inputText;

            // Создаем список для хранения всех элементов
            var allItems = dbSet.ToList();

            // Проверяем, существует ли элемент с таким же значением свойства
            var existingItem = allItems.FirstOrDefault(item => selector(item) == propertyValue);

            if (existingItem != null)
            {
                // Показываем сообщение об ошибке, если элемент уже существует
                ShowErrorMessage($"Такой {typeof(T).Name.ToLower()} уже существует!");
            }
            else
            {
                // Создаем новый экземпляр элемента типа T
                var newItem = new T();

                // Устанавливаем значение свойства Name<тип T> для нового элемента
                newItem.GetType().GetProperty(propertyName).SetValue(newItem, propertyValue);

                // Добавляем новый элемент в набор данных и сохраняем изменения в базе данных
                dbSet.Add(newItem);
                try
                {
                    context.SaveChanges();

                    // Создаем ObservableCollection для автоматического уведомления об изменениях
                    var observableCollection = new ObservableCollection<T>(allItems);

                    // Обновляем источник данных ComboBox, подписывая его на события изменения коллекции
                    comboBox.ItemsSource = observableCollection;

                    // Добавляем новый элемент в коллекцию
                    observableCollection.Add(newItem);

                    // Устанавливаем новый элемент как выбранный в ComboBox
                    comboBox.SelectedItem = newItem;
                }
                catch (DbEntityValidationException ex)
                {
                    // Обрабатываем ошибки валидации сущностей
                    HandleValidationErrors(ex);
                }
                catch (Exception ex)
                {
                    // Обрабатываем другие ошибки
                    Console.WriteLine($"An error occurred: {ex}");
                }
            }
        }

        private string ShowInputDialog(string message)
        {
            var inputWindow = new InputDialogWindow(message);
            return inputWindow.ShowDialog() == true ? inputWindow.InputText : null;
        }

        private void ShowErrorMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();
        }

        private void HandleValidationErrors(DbEntityValidationException ex)
        {
            foreach (var entityValidationErrors in ex.EntityValidationErrors)
            {
                foreach (var validationError in entityValidationErrors.ValidationErrors)
                {
                    Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                }
            }
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ElementsToolsClass.AllFieldsFilled(this))
                {
                    new MaterialDesignMessageBox("Не все поля заполнены!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                EditMedicineMethod();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void EditMedicineMethod()
        {
            try
            {
                var context = DBEntities.GetContext();
                var existingMedicine = context.Medicine.FirstOrDefault(m => m.IdMedicine == medicine.IdMedicine);

                if (existingMedicine != null)
                {
                    // Производим проверки, как в методе AddButton

                    if (ElementsToolsClass.AllFieldsFilled(this))
                    {
                        string dosageText = DosageTB.Text.Trim();
                        dosageText = dosageText.Replace(',', '.');

                        if (!decimal.TryParse(dosageText, NumberStyles.Number, CultureInfo.InvariantCulture, out var dosage))
                        {
                            ShowErrorMessage("Неверный формат дозировки");
                            return;
                        }

                        if (!int.TryParse(UnitsPerPackageTB.Text, out var unitsPerPackage))
                        {
                            ShowErrorMessage("Неверный формат количества единиц в упаковке");
                            return;
                        }

                        if (!ContainsOnlyLettersOrSpaces(NameMedicineTB.Text))
                        {
                            ShowErrorMessage("Некорректное наименование лекарства! Используйте только буквы.");
                            return;
                        }

                        // Обновляем фото только если была выбрана новая фотография
                        if (!string.IsNullOrEmpty(selectedFileName))
                        {
                            existingMedicine.MedicinePhoto = medicine.MedicinePhoto;
                        }

                        // Обновление привязок данных для TextBox
                        NameMedicineTB.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        DosageTB.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        UnitsPerPackageTB.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        InstructionsTB.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

                        // Обновление привязок данных для ComboBox
                        TypeMedicineCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                        ActiveSubstanceCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                        ReleaseFormCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                        BestBeforeDateCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                        PrescriptionDrugStatusCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                        ManufacturerCB.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();

                        context.SaveChanges();
                        new MaterialDesignMessageBox("Изменения успешно сохранены!", MessageType.Success, MessageButtons.Ok).ShowDialog();
                        Close();
                    }
                    else
                    {
                        new MaterialDesignMessageBox("Медикамент не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private bool ContainsOnlyLettersOrSpaces(string input)
        {
            return input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
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

        private void HandleException(Exception ex) => new MaterialDesignMessageBox($"Произошла ошибка: {ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
    }
}
