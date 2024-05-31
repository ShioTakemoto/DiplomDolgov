using DiplomDolgov.DataFolder;
using DiplomDolgov.WindowFolder.CustomMessageBox;
using DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder;
using DiplomDolgov.WindowFolder.PharmacistWindowFolder;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                new MaterialDesignMessageBox("Выберите комнату для редактирования!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var editWindow = new LittleTablesEditWindow(selectedRoom, "Комната");

            var mainMedicineWindow = Window.GetWindow(this) as MainMedicineWorkerMainWindow;
            if (mainMedicineWindow != null)
            {
                // Показываем затемняющий слой
                mainMedicineWindow.ShowOverlay2();
                editWindow.Closed += (s, args) => mainMedicineWindow.HideOverlay2();
            }

            editWindow.ShowDialog();
            LoadData();
            ListRoomDG.Items.Refresh();
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = ListRoomDG.SelectedItem as Room;

            if (selectedRoom == null)
            {
                new MaterialDesignMessageBox("Выберите комнату для удаления!", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var result = new MaterialDesignMessageBox($"Вы уверены что хотите удалить комнату {selectedRoom.RoomNumber}?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == true)
            {
                try
                {
                    DBEntities.GetContext().Room.Remove(selectedRoom);
                    DBEntities.GetContext().SaveChanges();
                    new MaterialDesignMessageBox("Комната успешно удалена", MessageType.Success, MessageButtons.Ok).ShowDialog();
                    LoadData();
                    ListRoomDG.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new MaterialDesignMessageBox($"{ex.Message}", MessageType.Error, MessageButtons.Ok).ShowDialog();
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
    }
}
