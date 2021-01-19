using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPI_AOI.DB.Table
{
    public class PanelResults
    {
        public string TableName = "PanelResults";
        public string ID = "_ID";
        public string ModelName = "Model_Name";
        public string LoadTime = "Load_Time";
        public string SumPadOfModel = "Sum_Pad";
        public string MachineResult = "Machine_Result";
        public string ConfirmResult = "Confirm_Result";
        public string RunningMode = "Running_Mode";
        public string SN = "SN";

    }
    public class ImageSaved
    {
        public string TableName = "ImageSaved";
        public string ID = "_ID";
        public string Type = "Type";
        public string TimeCapture = "Time_Capture";
        public string ROI = "ROI";
        public string FovID = "FOV_ID";
        public string ROIGerber = "ROI_Gerber";
        public string ImagePath = "Image_Path";

    }
    public class ErrorDetails
    {
        public string TableName = "PadErrorDetail";
        public string ID = "_ID";
        public string ModelName = "Model_Name";
        public string Time = "Time";
        public string Type = "Type";
        public string Component = "Component";
        public string PadID = "Pad_ID";
        public string FovID = "FOV_ID";
        public string ROIOnFov = "ROI_On_FOV";
        public string ROIOnGerber = "ROI_On_Gerber";
        public string MachineResult = "Machine_Result";
        public string ConfirmResult = "ConfirmResult";
        public string AreaMeasure = "Area_Measure";
        public string AreaHight = "Area_Hight";
        public string AreaLow = "Area_Low";
        public string ShiftXMeasure = "Shift_X_Measure";
        public string ShiftXHight = "Shift_X_Hight";
        public string ShiftYMeasure = "Shift_Y_Measure";
        public string ShiftYHight = "Shift_Y_Hight";
    }
}
