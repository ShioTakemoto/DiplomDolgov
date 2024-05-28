using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using Microsoft.Win32;
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
using System.Windows.Shapes;
using System.Linq.Expressions;

namespace DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AddStaffWindow.xaml
    /// </summary>
    public partial class AddStaffWindow : Window
    {
        public int? IdUser { get; set; } // Теперь может принимать null значения
        private string selectedFileName = "";
        private Staff Staff = new Staff();
        public event EventHandler AddedStaff;
        public AddStaffWindow()
        {
            InitializeComponent();
            UserCB.ItemsSource = DBEntities.GetContext().User.ToList();
            PostCB.ItemsSource = DBEntities.GetContext().Post.ToList();
            GenderCB.ItemsSource = DBEntities.GetContext().Gender.ToList();
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
        private void AddButton(object sender, RoutedEventArgs e)
        {
            try
            {

                // Проверка на максимальную длину номера телефона
                if (PhoneNumberStaffTB.Text.Length != 11)
                {
                    ShowErrorMessage("Номер телефона должен содержать ровно 11 символов");
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

                int? idUser = null;
                if (UserCB.SelectedItem != null)
                {
                    idUser = (int)UserCB.SelectedValue;
                }

                var newStaff = new Staff
                {
                    // Остальные свойства Staff...
                    IdUser = idUser
                };

                // Создание нового сотрудника
                var newerStaff = new Staff
                {
                    LastNameStaff = LastNameStaffTB.Text,
                    FirstNameStaff = FirstNameStaffTB.Text,
                    MiddleNameStaff = MiddleNameStaffTB.Text,
                    EmailStaff = EmailStaffTB.Text,
                    PhoneNumberStaff = PhoneNumberStaffTB.Text,
                    IdUser = idUser,
                    IdPost = Convert.ToInt32(PostCB.SelectedValue),
                    IdGender = Convert.ToInt32(GenderCB.SelectedValue),
                    StaffPhoto = !string.IsNullOrEmpty(selectedFileName) ? ImageClass.ConvertImageToByteArray(selectedFileName) : null
                };

                // Добавление нового сотрудника в базу данных
                DBEntities.GetContext().Staff.Add(newerStaff);
                DBEntities.GetContext().SaveChanges();
                ShowSuccessMessage("Сотрудник добавлен");
                ElementsToolsClass.ClearAllControls(this);
                selectedFileName = string.Empty;
                StaffPhoto.Source = null;
                AddedStaff?.Invoke(this, EventArgs.Empty);

            }
            catch(Exception ex)
            { 
                    ShowErrorMessage($"Ошибка:{ex}");
            }
        }

        // Обработчик события для добавления новой должности
        private void AddPost(object sender, RoutedEventArgs e)
        {
            // Создание окна для ввода новой должности
            var inputWindow = new InputDialogWindow("Введите новую должность");
            if (inputWindow.ShowDialog() == true)
            {
                // Получение введенного текста
                var inputText = inputWindow.InputText;
                if (string.IsNullOrEmpty(inputText))
                {
                    // Показ сообщения об ошибке, если поле пустое
                    ShowErrorMessage("Поле не должно быть пустым!");
                }
                else if (!ContainsOnlyLetters(inputText))
                {
                    // Показ сообщения об ошибке, если введены недопустимые символы
                    ShowErrorMessage("Недопустимые символы! Допускаются только буквы и пробелы.");
                }
                else
                {
                    // Добавление новой должности в ComboBox и базу данных
                    AddComboBoxItem<Post>(PostCB, inputText);
                }
            }
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

        // Метод проверки, содержит ли строка только буквы и пробелы
        private bool ContainsOnlyLetters(string input)
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

        // Метод для добавления элемента в ComboBox и базу данных
        private void AddComboBoxItem<T>(ComboBox comboBox, string inputText) where T : class
        {
            // Получение контекста базы данных
            var context = DBEntities.GetContext();
            // Получение набора данных для типа T
            var dbSet = context.Set<T>();
            // Формирование имени свойства для поиска
            var propertyName = $"Name{typeof(T).Name}";

            // Создание параметра выражения для типа T
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "item");
            // Создание выражения для доступа к свойству
            var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);
            // Создание выражения константы для inputText
            var constant = System.Linq.Expressions.Expression.Constant(inputText, typeof(string));
            // Создание выражения равенства для сравнения свойства с inputText
            var equality = System.Linq.Expressions.Expression.Equal(property, constant);
            // Создание лямбда-выражения для использования в запросе
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equality, parameter);

            // Выполнение запроса для проверки существования элемента в базе данных
            var existingItem = dbSet.FirstOrDefault(lambda);

            if (existingItem != null)
            {
                // Показ сообщения об ошибке, если элемент уже существует
                ShowErrorMessage($"Такой {typeof(T).Name.ToLower()} уже существует!");
            }
            else
            {
                // Создание нового элемента типа T
                var newItem = (T)Activator.CreateInstance(typeof(T));
                // Установка значения свойства для нового элемента
                newItem.GetType().GetProperty(propertyName).SetValue(newItem, inputText);

                // Добавление нового элемента в набор данных и сохранение изменений в базе данных
                dbSet.Add(newItem);
                context.SaveChanges();

                // Обновление источника данных для ComboBox и установка нового элемента как выбранного
                comboBox.ItemsSource = dbSet.ToList();
                comboBox.SelectedItem = newItem;
            }
        }
    }
}
