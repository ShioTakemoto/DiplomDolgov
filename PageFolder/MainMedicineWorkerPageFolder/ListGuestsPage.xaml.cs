using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Data.Entity;
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

namespace DiplomDolgov.PageFolder.MainMedicineWorkerPageFolder
{
    /// <summary>
    /// Логика взаимодействия для ListGuestsPage.xaml
    /// </summary>
    public partial class ListGuestsPage : Page
    {
        public ListGuestsPage()
        {
            InitializeComponent();
            LoadRooms();
            Search();
        }

        private void LoadRooms()
        {
            var rooms = DBEntities.GetContext().Room.ToList();
            rooms.Insert(0, new Room { IdRoom = 0, RoomNumber = "Все" });
            RoomCB.ItemsSource = rooms;
        }

        private void Search()
        {
            var context = DBEntities.GetContext();
            var searchText = SearchTB.Text.Trim().ToLower();
            var selectedRoomNumber = (RoomCB.SelectedItem as Room)?.RoomNumber;
            var query = context.Guests.Include(g => g.Gender).Include(g => g.Room).AsQueryable(); // Включение связанных данных

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(g =>
                    g.LastNameGuest.ToLower().Contains(searchText) ||
                    g.FirstNameGuest.ToLower().Contains(searchText) ||
                    g.MiddleNameGuest.ToLower().Contains(searchText) ||
                    g.PhoneNumberGuest.ToLower().Contains(searchText)
                );
            }

            if (!string.IsNullOrEmpty(selectedRoomNumber) && selectedRoomNumber != "Все")
            {
                query = query.Where(g =>
                    g.Room.RoomNumber.Equals(selectedRoomNumber)
                );
            }

            try
            {
                ListGuestsDG.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        private void DeleteM1_Click(object sender, RoutedEventArgs e)
        {
            var selectedGuest = ListGuestsDG.SelectedItem as Guests;

            if (selectedGuest == null)
            {
                new MaterialDesignMessageBox("Выберите гостя для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить {selectedGuest.LastNameGuest} {selectedGuest.FirstNameGuest}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    var context = DBEntities.GetContext();
                    context.Guests.Remove(selectedGuest);
                    context.SaveChanges();
                    new MaterialDesignMessageBox("Гость успешно удалён", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    RefreshDataGrid();
                    ListGuestsDG.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex}", MessageType.Error, MessageButtons.Ok).ShowDialog();
                }
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

            var context = DBEntities.GetContext();
            var guest = context.Guests.FirstOrDefault(g => g.IdGuest == selectedGuest.IdGuest);

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

            editGuestWindow.ShowDialog();

            if (mainMedicineWorkerWindow != null)
            {
                mainMedicineWorkerWindow.HideOverlay2();
            }

            RefreshDataGrid();
            ListGuestsDG.Items.Refresh();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void RoomCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Search();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addGuestWindow = new AddGuestWindow();

            var mainMedicineWorkerMainWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWorkerMainWindow != null)
            {
                mainMedicineWorkerMainWindow.ShowOverlay2();
                addGuestWindow.Closed += (s, args) => mainMedicineWorkerMainWindow.HideOverlay2();
            }
            addGuestWindow.AddedGuest += AddGuestWindow_AddedGuest;

            addGuestWindow.ShowDialog();
        }

        private void AddGuestWindow_AddedGuest(object sender, EventArgs e)
        {
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            var context = DBEntities.GetContext();
            ListGuestsDG.ItemsSource = context.Guests.Include(g => g.Gender).Include(g => g.Room).ToList();
        }
    }
}
