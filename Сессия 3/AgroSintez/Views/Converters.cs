using System;
using System.Windows.Data;
using System.Globalization;

namespace AgroSintez.Views
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == "in_progress") return "#F39C12";
            if (status == "completed") return "#27AE60";
            if (status == "planned") return "#3498DB";
            if (status == "rejected") return "#E74C3C";
            return "#2C3E50";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}