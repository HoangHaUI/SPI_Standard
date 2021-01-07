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
using System.Windows.Shapes;
using SPI_AOI.Models;
using System.Collections;


namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for ThreshWindow.xaml
    /// </summary>
    public partial class PadconditionWindow : Window
    {
        StandardThreshold mAreaThreshold = new StandardThreshold(120,80);
        StandardThreshold mVolumeThreshold = new StandardThreshold(120, 80);
        StandardThreshold mShiftXThreshold = new StandardThreshold(120, 80);
        StandardThreshold mShiftYThreshold = new StandardThreshold(120, 80);
        List<PadItem> mAllPads = new List<PadItem>();
        List<PadItem> mPadsSelected = new List<PadItem>();
        public PadconditionWindow(List<PadItem> AllPads, List<PadItem> PadSelected)
        {
            mAllPads = AllPads;
            mPadsSelected = PadSelected;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            LoadUI();
        }
        private void LoadUI()
        {
            List<double> areaUSL = new List<double>();
            List<double> areaLSL = new List<double>();
            List<double> shiftXUSL = new List<double>();
            List<double> shiftXLSL = new List<double>();
            List<double> shiftYUSL = new List<double>();
            List<double> shiftYLSL = new List<double>();
            foreach (var item in mPadsSelected)
            {
                if (!areaUSL.Contains(item.AreaThresh.UM_USL))
                    areaUSL.Add(item.AreaThresh.UM_USL);

                if (!areaLSL.Contains(item.AreaThresh.PERCENT_LSL))
                    areaLSL.Add(item.AreaThresh.PERCENT_LSL);

                if (!shiftXUSL.Contains(item.ShiftXThresh.UM_USL))
                    shiftXUSL.Add(item.ShiftXThresh.UM_USL);

                if (!shiftXLSL.Contains(item.ShiftXThresh.PERCENT_LSL))
                    shiftXLSL.Add(item.ShiftXThresh.PERCENT_LSL);

                if (!shiftYUSL.Contains(item.ShiftYThresh.UM_USL))
                    shiftYUSL.Add(item.ShiftYThresh.UM_USL);

                if (!shiftYLSL.Contains(item.ShiftYThresh.PERCENT_LSL))
                    shiftYLSL.Add(item.ShiftYThresh.PERCENT_LSL);
            }
            if (areaUSL.Count == 1)
                trAreaUSL.Value = areaUSL[0];
            if (areaLSL.Count == 1)
                trAreaLSL.Value = areaLSL[0];
            if (shiftXUSL.Count == 1)
                trShiftXUSL.Value = shiftXUSL[0];
            if (shiftXLSL.Count == 1)
                trShiftXLSL.Value = shiftXLSL[0];
            if (shiftYUSL.Count == 1)
                trShiftYUSL.Value = shiftYUSL[0];
            if (shiftYLSL.Count == 1)
                trShiftYLSL.Value = shiftYLSL[0];
        }
        private void trAreaUSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mAreaThreshold.UM_USL = s.Value;
        }

        private void trAreaLSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mAreaThreshold.PERCENT_LSL = s.Value;
        }
        private void trShiftXUSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mShiftXThreshold.UM_USL = s.Value;
        }
        private void trShiftXLSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mShiftXThreshold.PERCENT_LSL = s.Value;
        }
        private void trShiftYUSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mShiftYThreshold.UM_USL = s.Value;
        }

        private void trShiftYLSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mShiftYThreshold.PERCENT_LSL = s.Value;
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("Save Changed?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if(r == MessageBoxResult.Yes)
            {
                List<PadItem> items = (bool)rbAllPads.IsChecked ? mAllPads : mPadsSelected;
                foreach (var item in items)
                {
                    item.AreaThresh = mAreaThreshold;
                    item.ShiftXThresh = mShiftXThreshold;
                    item.ShiftYThresh = mShiftYThreshold;
                }
                MessageBox.Show("successfully!", "Question", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtAreaUSLVal_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                try
                {
                    TextBox txt = sender as TextBox;
                    int val = Convert.ToInt32(txt.Text);
                    trAreaUSL.Value = val;
                }
                catch { }
            }
        }

        private void txtAreaLSL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    TextBox txt = sender as TextBox;
                    int val = Convert.ToInt32(txt.Text);
                    trAreaLSL.Value = val;
                }
                catch { }
            }
        }

        private void txtShiftXUSL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    TextBox txt = sender as TextBox;
                    int val = Convert.ToInt32(txt.Text);
                    trShiftXUSL.Value = val;
                }
                catch { }
            }
        }

        private void txtShiftXLSL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    TextBox txt = sender as TextBox;
                    int val = Convert.ToInt32(txt.Text);
                    trShiftXLSL.Value = val;
                }
                catch { }
            }
        }

        private void txtShiftYUSL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    TextBox txt = sender as TextBox;
                    int val = Convert.ToInt32(txt.Text);
                    trShiftYUSL.Value = val;
                }
                catch { }
            }
        }

        private void txtShiftYLSL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    TextBox txt = sender as TextBox;
                    int val = Convert.ToInt32(txt.Text);
                    trShiftYLSL.Value = val;
                }
                catch { }
            }
        }
    }
}
