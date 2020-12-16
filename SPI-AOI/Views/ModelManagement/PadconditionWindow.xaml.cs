﻿using System;
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
            List<double> volumeUSL = new List<double>();
            List<double> volumeLSL = new List<double>();
            List<double> shiftXUSL = new List<double>();
            List<double> shiftXLSL = new List<double>();
            List<double> shiftYUSL = new List<double>();
            List<double> shiftYLSL = new List<double>();
            foreach (var item in mPadsSelected)
            {
                if (!areaUSL.Contains(item.AreaThresh.USL))
                    areaUSL.Add(item.AreaThresh.USL);

                if (!areaLSL.Contains(item.AreaThresh.LSL))
                    areaLSL.Add(item.AreaThresh.LSL);

                if (!volumeUSL.Contains(item.VolumeThresh.USL))
                    volumeUSL.Add(item.VolumeThresh.USL);

                if (!volumeLSL.Contains(item.VolumeThresh.LSL))
                    volumeLSL.Add(item.VolumeThresh.LSL);

                if (!shiftXUSL.Contains(item.ShiftXThresh.USL))
                    shiftXUSL.Add(item.ShiftXThresh.USL);

                if (!shiftXLSL.Contains(item.ShiftXThresh.LSL))
                    shiftXLSL.Add(item.ShiftXThresh.LSL);

                if (!shiftYUSL.Contains(item.ShiftYThresh.USL))
                    shiftYUSL.Add(item.ShiftYThresh.USL);

                if (!shiftYLSL.Contains(item.ShiftYThresh.LSL))
                    shiftYLSL.Add(item.ShiftYThresh.LSL);
            }
            if (areaUSL.Count == 1)
                trAreaUSL.Value = areaUSL[0];
            if (areaLSL.Count == 1)
                trAreaLSL.Value = areaLSL[0];
            if (volumeUSL.Count == 1)
                trVolumeUSL.Value = volumeUSL[0];
            if (volumeLSL.Count == 1)
                trVolumeLSL.Value = volumeLSL[0];
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
            mAreaThreshold.USL = s.Value;
        }

        private void trAreaLSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mAreaThreshold.LSL = s.Value;
        }
        private void trVolumeUSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mVolumeThreshold.USL = s.Value;
        }

        private void trVolumeLSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mVolumeThreshold.LSL = s.Value;
        }

        private void trShiftXUSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mShiftXThreshold.USL = s.Value;
        }
        private void trShiftXLSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mShiftXThreshold.LSL = s.Value;
        }
        private void trShiftYUSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mShiftYThreshold.USL = s.Value;
        }

        private void trShiftYLSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            s.Value = Math.Round(s.Value, 1);
            mShiftYThreshold.LSL = s.Value;
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
                    item.VolumeThresh = mVolumeThreshold;
                    item.ShiftXThresh = mShiftXThreshold;
                    item.ShiftYThresh = mShiftYThreshold;
                }
                MessageBox.Show("successfully!", "Question", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}