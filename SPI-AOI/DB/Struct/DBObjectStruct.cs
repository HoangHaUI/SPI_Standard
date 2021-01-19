using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SPI_AOI.DB.Struct
{
    public class ResultsObject
    {
        public string ID { get; set; }
        public string ModelName { get; set; }
        public string SN { get; set; }
        public DateTime LoadTime { get; set; }
        public string ConfirmResult { get; set; }
        public string MachineResult { get; set; }
        public string RunningMode { get; set; }

    }
    public class ImageSavedObject
    {
        public string TableName { get; set; }
        public string ID { get; set; }
        public string Type { get; set; }
        public DateTime TimeCapture { get; set; }
        public Rectangle ROI { get; set; }
        public int FovID { get; set; }
        public Rectangle ROIGerber { get; set; }
        public string ImagePath { get; set; }

    }
    public class PadErrorObject
    {
        public string ID { get; set; }
        public string ModelName { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string Component { get; set; }
        public int PadID { get; set; }
        public int FovID { get; set; }
        public Rectangle ROIOnFov { get; set; }
        public Rectangle ROIOnGerber { get; set; }
        public string MachineResult { get; set; }
        public string ConfirmResult { get; set; }
        public double AreaMeasure { get; set; }
        public double AreaHight { get; set; }
        public double AreaLow { get; set; }
        public double ShiftXMeasure { get; set; }
        public double ShiftXHight { get; set; }
        public double ShiftYMeasure { get; set; }
        public double ShiftYHight { get; set; }

    }
}
