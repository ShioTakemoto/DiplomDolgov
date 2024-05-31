using DiplomDolgov.DataFolder;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DiplomDolgov.WindowFolder.PharmacistWindowFolder
{
    public partial class LittleTablesEditWindow : Window
    {
        private object _record;
        private string _recordType;

        public LittleTablesEditWindow(object record, string recordType)
        {
            InitializeComponent();
            _record = record;
            _recordType = recordType;

            RecordLabel.Content = $"Редактирование записи: {_recordType}";
            EditTextBox.Text = GetRecordName(record);
        }

        private string GetRecordName(object record)
        {
            switch (_recordType)
            {
                case "Тип медикамента":
                    return ((TypeMedicine)record).NameTypeMedicine;
                case "Активное вещество":
                    return ((ActiveSubstance)record).NameActiveSubstance;
                case "Форма выпуска":
                    return ((ReleaseForm)record).NameReleaseForm;
                case "Срок годности":
                    return ((BestBeforeDate)record).NameBestBeforeDate;
                case "Страна производителя":
                    return ((ManufacturerCountry)record).NameManufacturerCountry;
                case "Комната":
                    return ((Room)record).RoomNumber;
                default:
                    return string.Empty;
            }
        }

        private void SaveRecordName(string newName)
        {
            switch (_recordType)
            {
                case "Тип медикамента":
                    ((TypeMedicine)_record).NameTypeMedicine = newName;
                    break;
                case "Активное вещество":
                    ((ActiveSubstance)_record).NameActiveSubstance = newName;
                    break;
                case "Форма выпуска":
                    ((ReleaseForm)_record).NameReleaseForm = newName;
                    break;
                case "Срок годности":
                    ((BestBeforeDate)_record).NameBestBeforeDate = newName;
                    break;
                case "Страна производителя":
                    ((ManufacturerCountry)_record).NameManufacturerCountry = newName;
                    break;
                case "Комната":
                    ((Room)_record).RoomNumber = newName;
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveRecordName(EditTextBox.Text);

            var context = DBEntities.GetContext();
            context.SaveChanges();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
