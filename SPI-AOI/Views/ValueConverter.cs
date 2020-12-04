using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

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

}
