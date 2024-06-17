using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AddMedicineWindow.xaml
    /// </summary>
    public partial class AddMedicineWindow : Window
    {
        private string selectedFileName = "";
        private Medicine Medicine = new Medicine();
        public event EventHandler AddedMedicine;
        public AddMedicineWindow()
        {
            InitializeComponent();
            InitializeComboBoxes();
        }

        private void InitializeComboBoxes()
        {
            ActiveSubstanceCB.ItemsSource = DBEntities.GetContext().ActiveSubstance.ToList();
            ReleaseFormCB.ItemsSource = DBEntities.GetContext().ReleaseForm.ToList();
            BestBeforeDateCB.ItemsSource = DBEntities.GetContext().BestBeforeDate.ToList();
            PrescriptionDrugStatusCB.ItemsSource = DBEntities.GetContext().PrescriptionDrugStatus.ToList();
            TypeMedicineCB.ItemsSource = DBEntities.GetContext().TypeMedicine.ToList();
            ManufacturerCB.ItemsSource = DBEntities.GetContext().Manufacturer.ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = ShowConfirmationMessage($"Вы уверены что хотите выйти?");

            if (Result.Value)
            {
                WindowAnimationHelper.CloseWindowWithFadeOut(this);
            }
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
                    Medicine.MedicinePhoto = ImageClass.ConvertImageToByteArray(selectedFileName);
                    ImPhoto.Source = ImageClass.ConvertByteArrayToImage(Medicine.MedicinePhoto);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"{ex}");
            }
        }

        private void AddButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ElementsToolsClass.AllFieldsFilled(this))
                {
                    string dosageText = DosageTB.Text.Trim();

                    // Замена всех запятых на точки в Dosage
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

                    // Проверка на допустимость символов в наименовании лекарства
                    if (!ContainsOnlyLettersOrSpaces(NameMedicineTB.Text))
                    {
                        ShowErrorMessage("Некорректное наименование лекарства! Используйте только буквы.");
                        return;
                    }

                    // Преобразование значений из комбобоксов
                    int idTypeMedicine = GetComboBoxItemId<TypeMedicine>(TypeMedicineCB);
                    int idActiveSubstance = GetComboBoxItemId<ActiveSubstance>(ActiveSubstanceCB);
                    int idManufacturer = GetComboBoxItemId<Manufacturer>(ManufacturerCB);
                    int idReleaseForm = GetComboBoxItemId<ReleaseForm>(ReleaseFormCB);
                    int idBestBeforeDate = GetComboBoxItemId<BestBeforeDate>(BestBeforeDateCB);
                    int idPrescriptionDrugStatus = GetComboBoxItemId<PrescriptionDrugStatus>(PrescriptionDrugStatusCB);

                    var existingMedicine = DBEntities.GetContext().Medicine.FirstOrDefault(m =>
                            m.NameMedicine == NameMedicineTB.Text &&
                            m.IdTypeMedicine == idTypeMedicine &&
                            m.IdActiveSubstance == idActiveSubstance &&
                            m.IdManufacturer == idManufacturer &&
                            m.IdReleaseForm == idReleaseForm &&
                            m.Dosage == dosage &&
                            m.IdBestBeforeDate == idBestBeforeDate &&
                            m.UnitsPerPackage == unitsPerPackage &&
                            m.Instructions == InstructionsTB.Text &&
                            m.IdPrescriptionDrugStatus == idPrescriptionDrugStatus);

                    if (existingMedicine != null)
                    {
                        ShowWarningMessage("Такой медикамент уже существует");
                    }
                    else
                    {
                        var newMedicine = new Medicine
                        {
                            NameMedicine = NameMedicineTB.Text,
                            IdTypeMedicine = idTypeMedicine,
                            IdActiveSubstance = idActiveSubstance,
                            IdManufacturer = idManufacturer,
                            IdReleaseForm = idReleaseForm,
                            Dosage = dosage,
                            IdBestBeforeDate = idBestBeforeDate,
                            UnitsPerPackage = unitsPerPackage,
                            Instructions = InstructionsTB.Text,
                            IdPrescriptionDrugStatus = idPrescriptionDrugStatus,
                            MedicinePhoto = !string.IsNullOrEmpty(selectedFileName) ? ImageClass.ConvertImageToByteArray(selectedFileName) : null
                        };

                        DBEntities.GetContext().Medicine.Add(newMedicine);
                        DBEntities.GetContext().SaveChanges();
                        ShowSuccessMessage("Медикамент добавлен");
                        ElementsToolsClass.ClearAllControls(this);
                        selectedFileName = string.Empty;
                        ImPhoto.Source = null;
                        AddedMedicine?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    ShowErrorMessage("Не все поля заполнены!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"{ex}");
            }
        }

        // Метод для получения идентификатора выбранного элемента ComboBox
        private int GetComboBoxItemId<T>(ComboBox comboBox) where T : class
        {
            // Получаем выбранный элемент ComboBox и приводим его к типу T
            var selectedItem = comboBox.SelectedItem as T;

            // Проверяем, что выбранный элемент не равен null
            if (selectedItem != null)
            {
                // Формируем имя свойства, которое содержит идентификатор (например, IdProduct для типа Product)
                var propertyName = $"Id{typeof(T).Name}";

                // Получаем значение свойства идентификатора выбранного элемента
                var propertyValue = selectedItem.GetType().GetProperty(propertyName).GetValue(selectedItem);

                // Приводим значение свойства к типу int и возвращаем его
                return (int)propertyValue;
            }

            // Возвращаем -1, если выбранный элемент равен null или не удалось получить идентификатор
            return -1;
        }

        private bool IsValidName(string input)
        {
            return !Regex.IsMatch(input, @"\d");
        }

        private bool ContainsOnlyLettersOrSpaces(string input)
        {
            return input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c));
        }

        private bool ShowConfirmationMessage(string message)
        {
            bool? result = new MaterialDesignMessageBox(message, MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
            return result ?? false;
        }

        private void ShowErrorMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();
        }

        private void ShowWarningMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Warning, MessageButtons.Ok).ShowDialog();
        }

        private void ShowSuccessMessage(string message)
        {
            new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();
        }

        // Метод для добавления элемента в ComboBox и базу данных
        private void AddComboBoxItem<T>(ComboBox comboBox, string inputText) where T : class
        {
            // Получаем контекст базы данных
            var context = DBEntities.GetContext();
            // Получаем набор данных для типа T
            var dbSet = context.Set<T>();
            // Формируем имя свойства, по которому будем искать дубликаты (например, для типа Product, свойство будет NameProduct)
            var propertyName = $"Name{typeof(T).Name}";

            // Получаем значение свойства для нового элемента
            var propertyValue = inputText;

            // Получаем все элементы из набора данных
            var allItems = dbSet.ToList();

            // Проверяем, существует ли элемент с таким же значением свойства
            var existingItem = allItems.FirstOrDefault(item => (string)item.GetType().GetProperty(propertyName).GetValue(item) == propertyValue);

            if (existingItem != null)
            {
                // Показываем сообщение об ошибке, если элемент уже существует
                ShowErrorMessage($"Такой {typeof(T).Name.ToLower()} уже существует!");
            }
            else
            {
                // Создаем новый экземпляр элемента типа T
                var newItem = (T)Activator.CreateInstance(typeof(T));
                // Устанавливаем значение свойства propertyName для нового элемента
                newItem.GetType().GetProperty(propertyName).SetValue(newItem, propertyValue);

                // Добавляем новый элемент в набор данных и сохраняем изменения в базе данных
                dbSet.Add(newItem);
                context.SaveChanges();

                // Создаем наблюдаемую коллекцию для автоматического уведомления о изменениях
                var observableCollection = new ObservableCollection<T>(allItems);

                // Устанавливаем обновленную наблюдаемую коллекцию как источник данных для ComboBox
                comboBox.ItemsSource = observableCollection;

                // Добавляем новый элемент в наблюдаемую коллекцию
                observableCollection.Add(newItem);

                // Устанавливаем новый элемент как выбранный в ComboBox
                comboBox.SelectedItem = newItem;
            }
        }

        private void AddTypeMedicine(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новый тип медикамента")
            {
                Topmost = true // Устанавливаем окно на передний план
            };
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                else if (!ContainsOnlyLettersOrSpaces(inputText))
                {
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы и пробелы.");
                }
                else
                {
                    AddComboBoxItem<TypeMedicine>(TypeMedicineCB, inputText);
                }
            }
        }

        private void AddActiveSubstance(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новое активное вещество")
            {
                Topmost = true // Устанавливаем окно на передний план
            };
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                else if (!ContainsOnlyLettersOrSpaces(inputText))
                {
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы и пробелы.");
                }
                else
                {
                    AddComboBoxItem<ActiveSubstance>(ActiveSubstanceCB, inputText);
                }
            }
        }

        private void AddReleaseForm(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новую форму выпуска")
            {
                Topmost = true // Устанавливаем окно на передний план
            };
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                else if (!ContainsOnlyLettersOrSpaces(inputText))
                {
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы и пробелы.");
                }
                else
                {
                    AddComboBoxItem<ReleaseForm>(ReleaseFormCB, inputText);
                }
            }
        }

        private void AddBestBeforeDate(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новый срок годности")
            {
                Topmost = true // Устанавливаем окно на передний план
            };
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                else if (!ContainsOnlyLettersOrSpaces(inputText))
                {
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы и пробелы.");
                }
                else
                {
                    AddComboBoxItem<BestBeforeDate>(BestBeforeDateCB, inputText);
                }
            }
        }
    }
}
