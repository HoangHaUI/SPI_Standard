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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SPI_AOI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.Series
                series = new System.Windows.Forms.DataVisualization.Charting.Series("YeildRate");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            chartYeildRate.Series.Add(series);
            chartYeildRate.BackColor = System.Drawing.Color.Transparent;
            chartYeildRate.Series["YeildRate"].Points.AddXY("100%",89);
            chartYeildRate.Series["YeildRate"].Points.AddXY("30%",22);
            chartYeildRate.Series["YeildRate"].Points[1].Color = System.Drawing.Color.Red;
            chartYeildRate.Series["YeildRate"].Points[0].Color = System.Drawing.Color.Green;
            chartYeildRate.Series["YeildRate"].Points[0].LegendText = "PASS";
            chartYeildRate.Series["YeildRate"].Points[1].LegendText = "FAIL";
            chartYeildRate.Titles.Add("Counter Chart");
            chartYeildRate.BackColor = System.Drawing.Color.Transparent;
            chartYeildRate.ChartAreas[0].BackColor = System.Drawing.Color.Transparent;
            chartYeildRate.Legends[0].BackColor = System.Drawing.Color.Transparent;
        }
    }
}
