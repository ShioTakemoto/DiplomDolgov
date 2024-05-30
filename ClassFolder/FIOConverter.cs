using DiplomDolgov.DataFolder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DiplomDolgov.ClassFolder
{
    public class FIOConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guests guest)
            {
                return $"{guest.LastNameGuest} {guest.FirstNameGuest[0]}. {guest.MiddleNameGuest[0]}.";
            }
            else if (value is Staff staff)
            {
                return $"{staff.LastNameStaff} {staff.FirstNameStaff[0]}. {staff.MiddleNameStaff[0]}.";
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
