using DiplomDolgov.ClassFolder;
using DiplomDolgov.DataFolder;
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
using System.Windows.Shapes;

namespace DiplomDolgov.WindowFolder.SharedWindowsFolder
{
    /// <summary>
    /// Логика взаимодействия для MedicineDetailsWindow.xaml
    /// </summary>
    public partial class MedicineDetailsWindow : Window
    {
        public MedicineDetailsWindow(Medicine selectedMedicine)
        {
            InitializeComponent();
            DataContext = selectedMedicine;
            if (selectedMedicine.MedicinePhoto != null)
            {
                MedicineImage.Source = ImageClass.ConvertByteArrayToImage(selectedMedicine.MedicinePhoto);
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
