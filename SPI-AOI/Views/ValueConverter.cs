using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using SPI_AOI.Models;

namespace SPI_AOI.Views
{
    public class BrushColorCvt :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color? color = value as System.Drawing.Color?;
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(color.Value.R, color.Value.G, color.Value.B));
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            return System.Drawing.Color.FromArgb(brush.Color.R, brush.Color.G, brush.Color.B);
        }
    }
    public class BrushPadCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int pad = (int)value;
            SolidColorBrush brush = new SolidColorBrush(Colors.Red);
            if (pad >= 0)
                brush = new SolidColorBrush(Colors.Blue);
            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            return System.Drawing.Color.FromArgb(brush.Color.R, brush.Color.G, brush.Color.B);
        }
    }
    public class RoundDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;
            return Math.Round(val, 2);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
