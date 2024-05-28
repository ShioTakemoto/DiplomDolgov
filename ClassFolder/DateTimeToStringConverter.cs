using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DiplomDolgov.ClassFolder
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Форматируем дату и время в требуемом формате
                return dateTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            }

            return value?.ToString(); // Если значение не является DateTime, просто возвращаем его строковое представление
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Метод не используется в односторонних конвертерах значений
        }
    }
}
