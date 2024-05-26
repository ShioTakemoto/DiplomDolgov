using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EditMedicineWindow.xaml
    /// </summary>
    public partial class EditMedicineWindow : Window
    {
        private string selectedFileName = "";
        private Medicine medicine;
        public EditMedicineWindow(Medicine medicine)
        {
            InitializeComponent();
            this.medicine = medicine;
            DataContext = this.medicine;
            ActiveSubstanceCB.ItemsSource = DBEntities.GetContext()
            .ActiveSubstance.ToList();
            ReleaseFormCB.ItemsSource = DBEntities.GetContext()
            .ReleaseForm.ToList();
            BestBeforeDateCB.ItemsSource = DBEntities.GetContext()
            .BestBeforeDate.ToList();
            PrescriptionDrugStatusCB.ItemsSource = DBEntities.GetContext()
            .PrescriptionDrugStatus.ToList();
            TypeMedicineCB.ItemsSource = DBEntities.GetContext()
            .TypeMedicine.ToList();
            ManufacturerCB.ItemsSource = DBEntities.GetContext()
            .Manufacturer.ToList();
        }
        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MaterialDesignMessageBox($"Вы уверены что хотите выйти?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result.Value)
            {
                this.Close();
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
                    medicine.MedicinePhoto = ImageClass.ConvertImageToByteArray(selectedFileName);
                    ImPhoto.Source = ImageClass.ConvertByteArrayToImage(medicine.MedicinePhoto);
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void AddTypeMedicine(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новый тип медикамента");
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
                    if (context.TypeMedicine.Any(tm => tm.NameTypeMedicine == inputText))
                    {
                        new MaterialDesignMessageBox("Такой тип медикамента уже существует!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                    else
                    {
                        var newTypeMedicine = new TypeMedicine { NameTypeMedicine = inputText };
                        DBEntities.GetContext().TypeMedicine.Add(newTypeMedicine);
                        DBEntities.GetContext().SaveChanges();

                        TypeMedicineCB.ItemsSource = DBEntities.GetContext().TypeMedicine.ToList();
                        TypeMedicineCB.SelectedItem = newTypeMedicine;
                    }
                }
            }
        }

        private void AddActiveSubstance(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новое активное вещество");
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
                    if (context.ActiveSubstance.Any(acs => acs.NameActiveSubstance == inputText))
                    {
                        new MaterialDesignMessageBox("Такое активное вещество уже существует!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                    else
                    {
                        var newActiveSubstance = new ActiveSubstance { NameActiveSubstance = inputText };
                        context.ActiveSubstance.Add(newActiveSubstance);
                        context.SaveChanges();

                        ActiveSubstanceCB.ItemsSource = context.ActiveSubstance.ToList();
                        ActiveSubstanceCB.SelectedItem = newActiveSubstance;
                    }
                }
            }
        }

        private void AddReleaseForm(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новую форму выпуска");
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
                    if (context.ReleaseForm.Any(rf => rf.NameReleaseForm == inputText))
                    {
                        new MaterialDesignMessageBox("Такая форма выпуска уже существует!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                    else
                    {
                        var newReleaseForm = new ReleaseForm { NameReleaseForm = inputText };
                        DBEntities.GetContext().ReleaseForm.Add(newReleaseForm);
                        DBEntities.GetContext().SaveChanges();

                        ReleaseFormCB.ItemsSource = DBEntities.GetContext().ReleaseForm.ToList();
                        ReleaseFormCB.SelectedItem = newReleaseForm;
                    }
                }
            }
        }

        private void AddBestBeforeDate(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новый срок годности");
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
                    if (DBEntities.GetContext().BestBeforeDate.Any(bb => bb.NameBestBeforeDate == inputText))
                    {
                        new MaterialDesignMessageBox("Такой срок годности уже существует!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                    else
                    {
                        var newBestBeforeDate = new BestBeforeDate { NameBestBeforeDate = inputText };
                        DBEntities.GetContext().BestBeforeDate.Add(newBestBeforeDate);
                        DBEntities.GetContext().SaveChanges();

                        BestBeforeDateCB.ItemsSource = DBEntities.GetContext().BestBeforeDate.ToList();
                        BestBeforeDateCB.SelectedItem = newBestBeforeDate;
                    }
                }
            }
        }

        private void SaveButton(object sender, RoutedEventArgs e)
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
                        new MaterialDesignMessageBox("Неверный формат дозировки", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    if (!int.TryParse(UnitsPerPackageTB.Text, out var unitsPerPackage))
                    {
                        new MaterialDesignMessageBox("Неверный формат количества единиц в упаковке", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    // Проверка на допустимость значений дозировки и количества единиц в упаковке
                    if (dosage <= 0 || unitsPerPackage <= 0)
                    {
                        new MaterialDesignMessageBox("Дозировка и количество единиц в упаковке должны быть положительными числами!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    // Проверка на допустимость символов в наименовании лекарства
                    if (!ContainsOnlyLetters(NameMedicineTB.Text))
                    {
                        new MaterialDesignMessageBox("Некорректное наименование лекарства! Используйте только буквы.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    int maxNameLength = 100; // Максимальная длина наименования
                    int maxInstructionsLength = 1000; // Максимальная длина инструкций

                    // Проверка на длину наименования и инструкций
                    if (NameMedicineTB.Text.Length > maxNameLength)
                    {
                        new MaterialDesignMessageBox($"Длина наименования лекарства не должна превышать {maxNameLength} символов.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }

                    if (InstructionsTB.Text.Length > maxInstructionsLength)
                    {
                        new MaterialDesignMessageBox($"Длина инструкций не должна превышать {maxInstructionsLength} символов.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                        return;
                    }
                    try
                    {
                        DBEntities.GetContext().SaveChanges();
                        new MaterialDesignMessageBox("Данные успешно сохранены", MessageType.Success, MessageButtons.Ok).ShowDialog();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    }
                }
                else
                {
                    new MaterialDesignMessageBox("Не все поля заполнены!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private bool IsValidName(string input)
        {
            return !Regex.IsMatch(input, @"\d");
        }

        private bool ContainsOnlyLetters(string input)
        {
            return input.All(char.IsLetter);
        }
    }
}
