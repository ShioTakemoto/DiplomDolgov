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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiplomDolgov.WindowFolder.MainMedicineWorkerWindowFolder
{
    /// <summary>
    /// Логика взаимодействия для StaffDetailsWindow.xaml
    /// </summary>
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

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
