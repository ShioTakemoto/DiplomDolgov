using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder
{
    public partial class StaffDetailsWindow : Window
    {
        public StaffDetailsWindow(Staff selectedStaff)
        {
            InitializeComponent();
            DataContext = selectedStaff;
            if (selectedStaff.StaffPhoto != null)
            {
                StaffImage.Source = ImageClass.ConvertByteArrayToImage(selectedStaff.StaffPhoto);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = (Storyboard)this.Resources["WindowFadeIn"];
            fadeInAnimation.Begin(this);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CollapseAndUnfold_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Topmost = true;
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.CanResize;
                Topmost = false;
            }
        }
    }
}
