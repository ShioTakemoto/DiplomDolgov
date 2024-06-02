using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DiplomDolgov.PageFolder.MainMedicineWorkerPageFolder
{
    /// <summary>
    /// Логика взаимодействия для ListRoomPage.xaml
    /// </summary>
    public partial class ListRoomPage : Page
    {
        public ListRoomPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["PageFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void LoadData()
        {
            var context = DBEntities.GetContext();

            ListRoomDG.ItemsSource = context.Room.ToList();
        }

        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = ListRoomDG.SelectedItem as Room;

            if (selectedRoom == null)
            {
                ShowErrorMessage("Выберите комнату для редактирования!");
                return;
            }

            var editWindow = new LittleTablesEditWindow(selectedRoom, "Комната");
            HandleOverlay(editWindow);

            editWindow.ShowDialog();
            LoadData();
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = ListRoomDG.SelectedItem as Room;

            if (selectedRoom == null)
            {
                ShowErrorMessage("Выберите комнату для удаления!");
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить комнату {selectedRoom.RoomNumber}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    DBEntities.GetContext().Room.Remove(selectedRoom);
                    DBEntities.GetContext().SaveChanges();
                    ShowSuccessMessage("Комната успешно удалена");
                    LoadData();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTB.Text.ToLower();

            var roomView = (CollectionView)CollectionViewSource.GetDefaultView(ListRoomDG.ItemsSource);
            roomView.Filter = item =>
            {
                var room = item as Room;
                return room.RoomNumber.ToLower().Contains(searchText);
            };
        }

        private void ShowErrorMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Error, MessageButtons.Ok).ShowDialog();

        private void ShowSuccessMessage(string message) => new MaterialDesignMessageBox(message, MessageType.Success, MessageButtons.Ok).ShowDialog();

        private void HandleOverlay(Window window)
        {
            var mainMedicineWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWindow != null)
            {
                mainMedicineWindow.ShowOverlay2();
                window.Closed += (s, args) => mainMedicineWindow.HideOverlay2();
            }
        }
    }
}
