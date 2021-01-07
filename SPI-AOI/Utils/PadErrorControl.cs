using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPI_AOI.Utils
{
    class PadErrorControl : Button
    {
        public int ID { get; set; }
        public Image Image { get; set; }
        public Label Label { get; set; }
        public PadErrorControl(System.Drawing.Bitmap image, int PadID)
        {
            Canvas cv = new Canvas();
            this.Width = 200;
            this.Height = 200;
            this.Background = Brushes.Transparent;
            this.BorderBrush = Brushes.Gray;
            this.BorderThickness = new Thickness(1);
            this.Margin = new Thickness(5);
            this.HorizontalContentAlignment = HorizontalAlignment.Left;
            this.VerticalContentAlignment = VerticalAlignment.Top;
            cv.Children.Add(GetImageControl(image));
            cv.Children.Add(GetLabel(PadID));
            this.AddChild(cv);
            this.ToolTip = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        }
        private Border GetImageControl(System.Drawing.Bitmap image)
        {
            this.Image = new Image();
            BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(image);
            this.Image.Source = bms;
            Border bd = new Border();
            bd.Margin = new Thickness(3, 30, 3, 3);
            bd.Width = 190;
            bd.Height = 160;
            bd.Background = Brushes.Gray;
            bd.ClipToBounds = true;
            bd.Child = this.Image;


            return bd;
        }
        private Label GetLabel(int PadID)
        {
            this.Label = new Label();
            this.Label.Content = "Pad ID:  " + PadID.ToString();
            this.Label.FontSize = 16;
            return this.Label;
        }
    }
}
