using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DiplomDolgov.ClassFolder
{
    public class StockToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stockCount)
            {
                // Задаем кисть в зависимости от значения StockCount
                if (stockCount <= 5)
                {
                    return new SolidColorBrush(Colors.Red); // Например, если StockCount меньше или равен 5, возвращаем красную кисть
                }
                else
                {
                    return new SolidColorBrush(Colors.Green); // Если StockCount больше 5, возвращаем зеленую кисть
                }
            }
            return new SolidColorBrush(Colors.Transparent); // По умолчанию возвращаем прозрачную кисть
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
