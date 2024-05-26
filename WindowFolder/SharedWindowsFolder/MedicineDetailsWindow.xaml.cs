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

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
