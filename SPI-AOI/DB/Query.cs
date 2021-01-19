using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Drawing;

namespace SPI_AOI.DB
{
    public class Query
    {
        private SQLiteConnection mConn = SQLiteConn.GetInstance();
        private GenCtl mCtl = new GenCtl();

        public Struct.ImageSavedObject[] GetImageSavedDetails(string ID)
        {
            List<Struct.ImageSavedObject> imageSavedObj = new List<Struct.ImageSavedObject>();
            Table.ImageSaved imageSaved = new Table.ImageSaved();
            string cmd = string.Format("Select * from {0} where {1} and {2}",
                imageSaved.TableName,
                imageSaved.ID + "=\'" + ID + "\'",
                imageSaved.Type + "=\'FOV\'");
            var r = mCtl.ExecuteReader(mConn, cmd);
            for (int i = 0; i < r.Count; i++)
            {
                RectangleConverter cvt = new RectangleConverter();
                var item = (Dictionary<string, object>)r[i];
                Struct.ImageSavedObject resObj = new Struct.ImageSavedObject();
                resObj.ID = Convert.ToString(item[imageSaved.ID]);
                resObj.TimeCapture = (DateTime)Convert.ChangeType(item[imageSaved.TimeCapture], typeof(DateTime));
                resObj.ImagePath = (string)item[imageSaved.ImagePath];
                resObj. ROI = (Rectangle)cvt.ConvertFromString((string)item[imageSaved.ROI]);
                resObj.ROIGerber = (Rectangle)cvt.ConvertFromString((string)item[imageSaved.ROIGerber]);
                resObj.FovID = Convert.ToInt32(item[imageSaved.FovID]);
                resObj.Type = (string)item[imageSaved.Type];
                imageSavedObj.Add(resObj);
            }
            return imageSavedObj.ToArray();


        }
        public Struct.PadErrorObject[] GetPadErrorDetails(string ID)
        {
            List<Struct.PadErrorObject> padErrorDetails = new List<Struct.PadErrorObject>();
            Table.ErrorDetails padErrorTbl = new Table.ErrorDetails();
            string cmd = string.Format("Select * from {0} where {1}",
                padErrorTbl.TableName,
                padErrorTbl.ID + "=\'" + ID + "\'");
            var r = mCtl.ExecuteReader(mConn, cmd);
            for (int i = 0; i < r.Count; i++)
            {
                RectangleConverter cvt = new RectangleConverter();
                var item = (Dictionary<string, object>)r[i];
                Struct.PadErrorObject resObj = new Struct.PadErrorObject();
                resObj.ID = Convert.ToString(item[padErrorTbl.ID]);
                resObj.Time = (DateTime)Convert.ChangeType(item[padErrorTbl.Time], typeof(DateTime));
                resObj.ModelName = (string)item[padErrorTbl.ModelName];
                resObj.Type = (string)item[padErrorTbl.Type];
                resObj.Component = (string)item[padErrorTbl.Component];
                resObj.FovID = Convert.ToInt32(item[padErrorTbl.FovID]);
                resObj.PadID = Convert.ToInt32(item[padErrorTbl.PadID]);
                resObj.ROIOnFov = (Rectangle)cvt.ConvertFromString((string)item[padErrorTbl.ROIOnFov]);
                resObj.ROIOnGerber = (Rectangle)cvt.ConvertFromString((string)item[padErrorTbl.ROIOnGerber]);
                resObj.MachineResult = (string)item[padErrorTbl.MachineResult];
                resObj.ConfirmResult = (string)item[padErrorTbl.ConfirmResult];
                resObj.AreaHight = Convert.ToDouble(item[padErrorTbl.AreaHight]);
                resObj.AreaMeasure = Convert.ToDouble(item[padErrorTbl.AreaMeasure]);
                resObj.AreaLow = Convert.ToDouble(item[padErrorTbl.AreaLow]);
                resObj.ShiftXHight = Convert.ToDouble(item[padErrorTbl.ShiftXHight]);
                resObj.ShiftXMeasure = Convert.ToDouble(item[padErrorTbl.ShiftXMeasure]);
                resObj.ShiftYHight = Convert.ToDouble(item[padErrorTbl.ShiftYHight]);
                resObj.ShiftYMeasure = Convert.ToDouble(item[padErrorTbl.ShiftYMeasure]);
                padErrorDetails.Add(resObj);
            }
            return padErrorDetails.ToArray();
        }

        public Struct.ResultsObject[] GetResult(string ModelName, string SN, DateTime StartTime, DateTime EndTime)
        {
            List<Struct.ResultsObject> resultObj = new List<Struct.ResultsObject>();
            Table.PanelResults panelResultTbl = new Table.PanelResults();
            string stTime = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            string cmd = string.Format("Select * from {0}",
                panelResultTbl.TableName);
            if(ModelName != "*")
            {
                if(!cmd.Contains("where"))
                {
                    cmd += " where ";
                }
                cmd += panelResultTbl.ModelName + "=\'" + ModelName + "\'";
            }
            if (SN != "*")
            {
                if (!cmd.Contains("where"))
                {
                    cmd += " where ";
                }
                else
                {
                    cmd += " and ";
                }
                cmd += panelResultTbl.SN + " like \'%" + SN + "%\'";
            }
            if (!cmd.Contains("where"))
            {
                cmd += " where ";
            }
            else
            {
                cmd += " and ";
            }
            cmd += panelResultTbl.LoadTime + ">\'" + stTime + "\'";
            cmd += " and ";
            cmd += panelResultTbl.LoadTime + "<=\'" + endTime + "\'";
            var r = mCtl.ExecuteReader(mConn, cmd);
            for (int i = 0; i < r.Count; i++)
            {
                var item = (Dictionary<string, object>)r[i];
                Struct.ResultsObject resObj = new Struct.ResultsObject();
                resObj.ID = Convert.ToString(item[panelResultTbl.ID]);
                resObj.LoadTime = (DateTime)Convert.ChangeType(item[panelResultTbl.LoadTime], typeof(DateTime)); ;
                resObj.SN = (string)item[panelResultTbl.SN];
                resObj.ModelName = (string)item[panelResultTbl.ModelName];
                resObj.RunningMode = (string)item[panelResultTbl.RunningMode];
                resObj.ConfirmResult = (string)item[panelResultTbl.ConfirmResult];
                resObj.MachineResult = (string)item[panelResultTbl.MachineResult];
                resultObj.Add(resObj);
            }
            return resultObj.ToArray();
        }
    }
}
