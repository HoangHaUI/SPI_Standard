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
        public Border Border { get; set; }
        public PadErrorControl(System.Drawing.Bitmap image, int PadID)
        {
            Canvas cv = new Canvas();
            this.Width = 190;
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
        public void SetStatus(int status = -1)
        {
            this.Dispatcher.Invoke(() => {
                if (status == -1)
                    this.Border.BorderBrush = Brushes.Red;
                else
                {
                    this.Border.BorderBrush = Brushes.Green;
                }
            });
        }
        private Border GetImageControl(System.Drawing.Bitmap image)
        {
            this.Image = new Image();
            BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(image);
            this.Image.Source = bms;
            this.Border = new Border();
            this.Border.Margin = new Thickness(3, 30, 3, 3);
            this.Border.Width = 180;
            this.Border.Height = 160;
            this.Border.Background = Brushes.Gray;
            this.Border.ClipToBounds = true;
            this.Border.Child = this.Image;
            this.Border.BorderBrush = Brushes.Red;
            this.Border.BorderThickness = new Thickness(1);
            return this.Border;
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
