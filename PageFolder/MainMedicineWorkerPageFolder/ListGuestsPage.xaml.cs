using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DiplomDolgov.PageFolder.MainMedicineWorkerPageFolder
{
    /// <summary>
    /// Логика взаимодействия для ListGuestsPage.xaml
    /// </summary>
    public partial class ListGuestsPage : Page
    {
        private string searchText = string.Empty;
        private string selectedRoomNumber = string.Empty;

        public ListGuestsPage()
        {
            InitializeComponent();
            LoadRooms();
            LoadGuests();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["PageFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadRooms()
        {
            var rooms = DBEntities.GetContext().Room.ToList();
            rooms.Insert(0, new Room { IdRoom = 0, RoomNumber = "Все" });
            RoomCB.ItemsSource = rooms;
        }

        private void LoadGuests()
        {
            var guestsList = DBEntities.GetContext().Guests.Include(g => g.Gender).Include(g => g.Room).ToList();
            ListGuestsDG.ItemsSource = guestsList;
            FilterGuests();
        }

        private void FilterGuests()
        {
            var context = DBEntities.GetContext();
            var filteredGuests = context.Guests.Include(g => g.Gender).Include(g => g.Room).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredGuests = filteredGuests.Where(g =>
                    g.LastNameGuest.ToLower().Contains(searchText) ||
                    g.FirstNameGuest.ToLower().Contains(searchText) ||
                    g.MiddleNameGuest.ToLower().Contains(searchText) ||
                    g.PhoneNumberGuest.ToLower().Contains(searchText)
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedRoomNumber) && selectedRoomNumber != "Все")
            {
                filteredGuests = filteredGuests.Where(g => g.Room.RoomNumber == selectedRoomNumber);
            }

            try
            {
                ListGuestsDG.ItemsSource = filteredGuests.ToList();
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"Ошибка: {ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedGuest = ListGuestsDG.SelectedItem as Guests;

                if (selectedGuest == null)
                {
                    new MaterialDesignMessageBox("Выберите гостя для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                var guest = DBEntities.GetContext().Guests.FirstOrDefault(g => g.IdGuest == selectedGuest.IdGuest);

                if (guest == null)
                {
                    new MaterialDesignMessageBox("Гость не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                    return;
                }

                var fioConverter = new DiplomDolgov.ClassFolder.FIOConverter();
                string fullName = fioConverter.Convert(guest, typeof(string), null, CultureInfo.CurrentCulture) as string;

                bool? result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {fullName}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

                if (result == true)
                {
                    DBEntities.GetContext().Guests.Remove(guest);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Гость успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadGuests();
                    ListGuestsDG.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"Ошибка: {ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void EditM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedGuest = ListGuestsDG.SelectedItem as Guests;

            if (selectedGuest == null)
            {
                new MaterialDesignMessageBox("Выберите гостя для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var guest = DBEntities.GetContext().Guests.FirstOrDefault(g => g.IdGuest == selectedGuest.IdGuest);

            if (guest == null)
            {
                new MaterialDesignMessageBox("Гость не найден!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editGuestWindow = new EditGuestWindow(guest);

            var mainMedicineWorkerWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWorkerWindow != null)
            {
                mainMedicineWorkerWindow.ShowOverlay2();
                editGuestWindow.Closed += (s, args) => mainMedicineWorkerWindow.HideOverlay2();
            }
            editGuestWindow.Topmost = true; // Устанавливаем на передний план
            editGuestWindow.ShowDialog();
            editGuestWindow.Activate(); // Активируем окно
            LoadGuests();
            ListGuestsDG.Items.Refresh();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = SearchTB.Text.Trim().ToLower();
            FilterGuests();
        }

        private void RoomCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRoomNumber = (RoomCB.SelectedItem as Room)?.RoomNumber ?? string.Empty;
            FilterGuests();
        }

        private void AddGuestWindow_AddedGuest(object sender, EventArgs e)
        {
            LoadGuests();
        }

        private void AddGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            var addGuestWindow = new AddGuestWindow();

            // Получаем ссылку на главное окно
            var mainMedicineWorkerMainWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWorkerMainWindow != null)
            {
                // Показываем затемняющий слой
                mainMedicineWorkerMainWindow.ShowOverlay2();
                addGuestWindow.Closed += (s, args) => mainMedicineWorkerMainWindow.HideOverlay2();
            }

            addGuestWindow.AddedGuest += AddGuestWindow_AddedGuest;
            addGuestWindow.Topmost = true; // Устанавливаем на передний план
            addGuestWindow.ShowDialog();
            addGuestWindow.Activate(); // Активируем окно
        }
    }
}
