using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    public partial class EditMedicineWindow : Window
    {
        private string selectedFileName = "";
        private Medicine medicine;

        public EditMedicineWindow(Medicine medicine)
        {
            InitializeComponent();
            this.medicine = medicine;
            DataContext = this.medicine;
            LoadData();
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
                Close();
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

        private void AddNewItem<T>(string message, ComboBox comboBox, Func<T, string> selector) where T : class, new()
        {
            var inputWindow = new InputDialogWindow(message);
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
                    if (context.Set<T>().Any(item => selector(item) == inputText))
                    {
                        new MaterialDesignMessageBox($"Такой {typeof(T).Name} уже существует!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                    else
                    {
                        var newItem = Activator.CreateInstance<T>();
                        var property = typeof(T).GetProperty(selector(newItem));
                        property.SetValue(newItem, inputText);
                        context.Set<T>().Add(newItem);
                        context.SaveChanges();

                        comboBox.ItemsSource = context.Set<T>().ToList();
                        comboBox.SelectedItem = newItem;
                    }
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

                var context = DBEntities.GetContext();
                var existingMedicine = context.Medicine.FirstOrDefault(m => m.IdMedicine == medicine.IdMedicine);
                if (existingMedicine != null)
                {
                    

                    // Обновляем фото только если была выбрана новая фотография
                    if (!string.IsNullOrEmpty(selectedFileName))
                    {
                        existingMedicine.MedicinePhoto = medicine.MedicinePhoto;
                    }

                    context.SaveChanges();
                    new MaterialDesignMessageBox("Изменения успешно сохранены!", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    Close();
                }
                else
                {
                    new MaterialDesignMessageBox("Медикамент не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private bool IsValidName(string input) => !Regex.IsMatch(input, @"\d");

        private void HandleException(Exception ex) => new MaterialDesignMessageBox($"Произошла ошибка: {ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
    }
}
