﻿using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для EditStaffWindow.xaml
    /// </summary>
    public partial class EditStaffWindow : Window
    {
        private Staff staff;
        private string selectedFileName = "";
        public EditStaffWindow(Staff staff)
        {
            InitializeComponent();
            this.staff = staff;
            DataContext = this.staff;
            UserCB.ItemsSource = DBEntities.GetContext().User.ToList();
            PostCB.ItemsSource = DBEntities.GetContext().Post.ToList();
            GenderCB.ItemsSource = DBEntities.GetContext().Gender.ToList();
        }
            

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на максимальную длину номера телефона
                if (PhoneNumberStaffTB.Text.Length > 11)
                {
                    ShowErrorMessage("Номер телефона не может содержать более 11 символов");
                    return;
                }

                if (PhoneNumberStaffTB.Text.Length < 11)
                {
                    ShowErrorMessage("Номер телефона не может содержать менее 11 символов");
                    return;
                }

                // Проверка, что номер телефона состоит только из цифр
                if (!PhoneNumberStaffTB.Text.All(char.IsDigit))
                {
                    ShowErrorMessage("Номер телефона должен состоять только из цифр");
                    return;
                }

                // Проверка на заполненность поля "Фамилия"
                if (string.IsNullOrEmpty(LastNameStaffTB.Text))
                {
                    ShowErrorMessage("Введите фамилию сотрудника");
                    return;
                }

                // Проверка на заполненность поля "Имя"
                if (string.IsNullOrEmpty(FirstNameStaffTB.Text))
                {
                    ShowErrorMessage("Введите имя сотрудника");
                    return;
                }

                // Проверка на заполненность поля "Отчество"
                if (string.IsNullOrEmpty(MiddleNameStaffTB.Text))
                {
                    ShowErrorMessage("Введите отчество сотрудника");
                    return;
                }

                // Проверка, что фамилия, имя и отчество содержат только буквы
                if (!ContainsOnlyLetters(LastNameStaffTB.Text) || !ContainsOnlyLetters(FirstNameStaffTB.Text) || !ContainsOnlyLetters(MiddleNameStaffTB.Text))
                {
                    ShowErrorMessage("Фамилия, имя и отчество должны содержать только буквы");
                    return;
                }

                int idUser = -1;
                if (UserCB.SelectedItem != null)
                {
                    idUser = (int)UserCB.SelectedValue;
                }

                // Получение сотрудника из базы данных для редактирования
                var context = DBEntities.GetContext();
                var existingStaff = context.Staff.FirstOrDefault(m => m.IdStaff == staff.IdStaff);
                if (existingStaff != null)
                {
                    if (!string.IsNullOrEmpty(selectedFileName))
                    {
                        existingStaff.StaffPhoto = staff.StaffPhoto;
                    }

                    context.SaveChanges();
                    ShowSuccessMessage("Данные сотрудника обновлены");
                    Close();
                }
                else
                {
                    ShowErrorMessage("Сотрудник не найден");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при редактировании сотрудника: {ex.Message}");
            }
        }
        private bool ContainsOnlyLetters(string input)
        {
            return input.All(char.IsLetter);
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
                    Staff.StaffPhoto = ImageClass.ConvertImageToByteArray(selectedFileName);
                    StaffPhoto.Source = ImageClass.ConvertByteArrayToImage(Staff.StaffPhoto);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"{ex}");
            }
        }

        private void AddPost(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputDialogWindow("Введите новую должность");
            if (inputWindow.ShowDialog() == true)
            {
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                else if (!ContainsOnlyLetters(inputText))
                {
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы.");
                }
                else
                {
                    AddComboBoxItem<Post>(PostCB, inputText);
                }
            }
        }

        private void AddComboBoxItem<T>(ComboBox comboBox, string inputText) where T : class
        {
            var context = DBEntities.GetContext();
            var dbSet = context.Set<T>();
            var propertyName = $"Name{typeof(T).Name}";

            var existingItem = dbSet.FirstOrDefault(item => (string)item.GetType().GetProperty(propertyName).GetValue(item) == inputText);

            if (existingItem != null)
            {
                ShowErrorMessage($"Такой {typeof(T).Name.ToLower()} уже существует!");
            }
            else
            {
                var newItem = (T)Activator.CreateInstance(typeof(T));
                newItem.GetType().GetProperty(propertyName).SetValue(newItem, inputText);

                dbSet.Add(newItem);
                context.SaveChanges();

                comboBox.ItemsSource = dbSet.ToList();
                comboBox.SelectedItem = newItem;
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowConfirmationMessage("Вы уверены что хотите выйти?"))
            {
                this.Close();
            }
        }

        private bool ShowConfirmationMessage(string message)
        {
            bool? result = new MaterialDesignMessageBox(message, MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
            return result ?? false;
        }
    }
}