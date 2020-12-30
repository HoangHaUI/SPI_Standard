using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPI_AOI.DB.Table
{
    public class Result
    {
        public string TableName = "Results";
        public string ID = "_ID";
        public string ModelName = "Model";
        public string LoadTime = "Load_Time";
        public string VIResult = "VI_Result";
        public string RunningMode = "Running_Mode";
        public string SN = "SN";

    }
    public class ImageSaved
    {
        public string TableName = "Images";
        public string Type = "Type";
        public string TimeCapture = "Time_Capture";
        public string ID = "_ID";
        public string ROI = "ROI";
        public string ROIGerber = "ROI_Gerber";
        public string ImagePath = "Image_Path";

    }
    public class ErrorDetails
    {
        public string TableName = "ErrorDetails";
        public string ID = "_ID";
        public string Time = "Time";
        public string Type = "Type";
        public string Component = "Component";
    }
}
