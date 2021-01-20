using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SPI_AOI.DB
{
    class Result
    {
        private SQLiteConnection mConn = SQLiteConn.GetInstance();
        private GenCtl mCtl = new GenCtl();
        public Result()
        {
            InitPanelResultTbl();
            InitImageTbl();
            InitErrorDetailsTbl();
        }
        public void InitPanelResultTbl()
        {
            Table.PanelResults resultTbl = new Table.PanelResults();
            string cmd = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1});",
                resultTbl.TableName,
                resultTbl.ID + " TEXT PRIMARY KEY," +
                resultTbl.ModelName + " TEXT," +
                resultTbl.LoadTime + " TEXT," +
                resultTbl.SN + " TEXT," +
                resultTbl.RunningMode + " TEXT," +
                resultTbl.MachineResult + " TEXT," +
                resultTbl.ConfirmResult + " TEXT," +
                resultTbl.SumPadOfModel + " INTEGER"
                );
            mCtl.ExecuteCmd(mConn, cmd);
        }
        public void InitImageTbl()
        {
            Table.ImageSaved imgTbl = new Table.ImageSaved();
            string cmd = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1});",
                imgTbl.TableName,
                imgTbl.ID + " TEXT," +
                imgTbl.TimeCapture + " TEXT," +
                imgTbl.ImagePath + " TEXT," +
                imgTbl.ROI + " TEXT," +
                imgTbl.ROIGerber + " TEXT," +
                imgTbl.FovID + " INTEGER," +
                imgTbl.Type + " TEXT"
                );
            mCtl.ExecuteCmd(mConn, cmd);
        }
        public void InitErrorDetailsTbl()
        {
            Table.ErrorDetails imgTbl = new Table.ErrorDetails();
            string cmd = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1});",
                imgTbl.TableName,
                imgTbl.ID + " TEXT," +
                imgTbl.ModelName + " TEXT," +
                imgTbl.Time + " TEXT," +
                imgTbl.Component + " TEXT," +
                imgTbl.PadID + " INTEGER," +
                imgTbl.FovID + " INTEGER," +
                imgTbl.ROIOnFov + " TEXT," +
                imgTbl.ROIOnGerber + " TEXT," +
                imgTbl.MachineResult + " TEXT," +
                imgTbl.ConfirmResult + " TEXT," +
                imgTbl.Type + " TEXT," +
                imgTbl.AreaMeasure + " REAL," +
                imgTbl.AreaHight + " REAL," +
                imgTbl.AreaLow + " REAL," +
                imgTbl.ShiftXMeasure + " REAL," +
                imgTbl.ShiftXHight + " REAL," +
                imgTbl.ShiftYMeasure + " REAL," +
                imgTbl.ShiftYHight + " REAL"
                );
            mCtl.ExecuteCmd(mConn, cmd);
        }
        public string GetNewID()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }
        public int InsertNewPanelResult(
            string ID,
            string ModelName,
            DateTime Time,
            string SN,
            string RunningMode,
            string MachineResult,
            string ConfirmResult,
            int SumOfPad
            )
        {
            Table.PanelResults resultTbl = new Table.PanelResults();
            string cmd = string.Format("INSERT INTO {0} ({1}) values({2});",
                resultTbl.TableName,
                // --------------
                resultTbl.ID + "," +
                resultTbl.ModelName + "," +
                resultTbl.LoadTime + "," +
                resultTbl.SN + "," +
                resultTbl.RunningMode + "," +
                resultTbl.MachineResult + "," +
                resultTbl.ConfirmResult + "," +
                resultTbl.SumPadOfModel,

                //--------------
                "\'" + ID  + "\'," +
                "\'" + ModelName + "\'," +
                "\'" + Time.ToString("yyyy-MM-dd HH:mm:ss") + "\'," +
                "\'" + SN + "\'," +
                "\'" + RunningMode + "\'," +
                "\'" + MachineResult + "\'," +
                "\'" + ConfirmResult + "\'," +
                "" + SumOfPad
                );
            return mCtl.ExecuteCmd(mConn, cmd);
        }
        public int InsertNewImage(
                string ID,
                DateTime TimeCapture,
                string ImagePath,
                int FovID,
                System.Drawing.Rectangle ROI,
                System.Drawing.Rectangle ROIGerber,
                string Type
            )
        {
            Table.ImageSaved resultTbl = new Table.ImageSaved();
            string cmd = string.Format("INSERT INTO {0} ({1}) values({2});",
                resultTbl.TableName,
                // --------------
                resultTbl.ID + "," +
                resultTbl.TimeCapture + "," +
                resultTbl.ImagePath + "," +
                resultTbl.FovID + "," +
                resultTbl.ROI + "," +
                resultTbl.ROIGerber + "," +
                resultTbl.Type,

                //--------------
                "\'" + ID + "\'," +
                "\'" + TimeCapture.ToString("yyyy-MM-dd HH:mm:ss") + "\'," +
                "\'" + ImagePath + "\'," +
                Convert.ToString(FovID) + "," + 
                "\'" + string.Format("{0},{1},{2},{3}",ROI.X, ROI.Y, ROI.Width, ROI.Height) + "\'," +
                "\'" + string.Format("{0},{1},{2},{3}", ROIGerber.X, ROIGerber.Y, ROIGerber.Width, ROIGerber.Height) + "\'," +
                "\'" + Type + "\'"
                );
            return mCtl.ExecuteCmd(mConn, cmd);
        }
        public int InsertNewPadError(
                string ID,
                string ModelName,
                DateTime Time,
                int PadID,
                int FovID,
                System.Drawing.Rectangle ROIOnFOV,
                System.Drawing.Rectangle ROIOnGerber,
                string MachineResult,
                string ConfirmResult,
                string Component,
                string Type,
                double AreaMeasure,
                double AreaHight,
                double AreaLow,
                double ShiftXMeasure,
                double ShiftXHight,
                double ShiftYMeasure,
                double ShiftYHight
            )
        {
            Table.ErrorDetails resultTbl = new Table.ErrorDetails();
            string cmd = string.Format("INSERT INTO {0} ({1}) values({2});",
                resultTbl.TableName,
                // --------------
                resultTbl.ID + "," +
                resultTbl.ModelName + "," +
                resultTbl.Time + "," +
                resultTbl.Component + "," +
                resultTbl.PadID + "," +
                resultTbl.FovID + "," +
                resultTbl.ROIOnFov + "," +
                resultTbl.ROIOnGerber + "," +
                resultTbl.MachineResult + "," +
                resultTbl.ConfirmResult + "," +
                resultTbl.Type + "," +
                resultTbl.AreaMeasure + "," +
                resultTbl.AreaHight + "," +
                resultTbl.AreaLow + "," +
                resultTbl.ShiftXMeasure + "," +
                resultTbl.ShiftXHight + "," +
                resultTbl.ShiftYMeasure + "," +
                resultTbl.ShiftYHight,

                //--------------
                "\'" + ID + "\'," +
                "\'" + ModelName + "\'," +
                "\'" + Time.ToString("yyyy-MM-dd HH:mm:ss") + "\'," +
                "\'" + Component + "\'," +
                "" + PadID + "," +
                "" + FovID + "," +
                "\'" + string.Format("{0},{1},{2},{3}", ROIOnFOV.X, ROIOnFOV.Y, ROIOnFOV.Width, ROIOnFOV.Height) + "\'," +
                "\'" + string.Format("{0},{1},{2},{3}", ROIOnGerber.X, ROIOnGerber.Y, ROIOnGerber.Width, ROIOnGerber.Height) + "\'," +
                "\'" + MachineResult + "\'," +
                "\'" + ConfirmResult + "\'," +
                "\'" + Type + "\'," +
                 + AreaMeasure + "," +
                 + AreaHight + "," +
                 + AreaLow + "," +
                 + ShiftXMeasure + "," +
                 + ShiftXHight + "," +
                 + ShiftYMeasure + "," +
                 + ShiftYHight
                );
            return mCtl.ExecuteCmd(mConn, cmd);
        }
        public int GetSumPadOfModel(string ModelName)
        {
            Table.PanelResults resultTbl = new Table.PanelResults();
            string cmd = string.Format("Select {0} from {1} where {2}",
                resultTbl.SumPadOfModel,
                resultTbl.TableName,
                resultTbl.ModelName + "=\'" + ModelName + "\'");
            object val = mCtl.ExecuteScalarCmd(mConn, cmd);
            return Convert.ToInt32(val);
        }
        public string[] GetModelName()
        {
            List<string> modelNames = new List<string>();
            Table.PanelResults resultTbl = new Table.PanelResults();
            string cmd = string.Format("SELECT {0} from {1};",
                resultTbl.ModelName,
                resultTbl.TableName);
            var reader = mCtl.ExecuteReader(mConn, cmd);
            for (int i = 0; i < reader.Count; i++)
            {
                Dictionary<string, object> item = (Dictionary<string, object>)reader[i];
                string modelName = (string)item[resultTbl.ModelName];
                if (!modelNames.Contains(modelName))
                {
                    modelNames.Add(modelName);
                }
            }
            return modelNames.ToArray();
        }
        public int CountPass(string ModelName, DateTime StartTime, DateTime EndTime)
        {
            Table.PanelResults resultTbl = new Table.PanelResults();
            string stTime = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            string cmd = string.Format("Select count({0}) from {1} where {2} and {3} and {4} and {5};",
                resultTbl.ID,
                resultTbl.TableName,
                resultTbl.ModelName + "=\'" + ModelName + "\'",
                resultTbl.LoadTime + ">\'" + stTime + "\'",
                resultTbl.LoadTime + "<=\'" + endTime + "\'",
                resultTbl.ConfirmResult + "=\'PASS\'"
                );

            object count = mCtl.ExecuteScalarCmd(mConn, cmd);
            return Convert.ToInt32(count);
        }
        public int CountDefect(string ModelName, string Key, DateTime StartTime, DateTime EndTime)
        {
            Table.ErrorDetails errorDetailsTbl = new Table.ErrorDetails();
            string stTime = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            string cmd = string.Format("Select count({0}) from {1} where {2} and {3} and {4} and {5} and {6};",
                errorDetailsTbl.ID,
                errorDetailsTbl.TableName,
                errorDetailsTbl.ModelName + "=\'" + ModelName + "\'",
                errorDetailsTbl.Time + ">\'" + stTime + "\'",
                errorDetailsTbl.Time + "<=\'" + endTime + "\'",
                errorDetailsTbl.Type + "=\'"+ Key + "\'",
                errorDetailsTbl.ConfirmResult + "=\'" + "FAIL" + "\'"
                );

            object count = mCtl.ExecuteScalarCmd(mConn, cmd);
            return Convert.ToInt32(count);
        }
        public int CountFail(string ModelName, DateTime StartTime, DateTime EndTime)
        {
            Table.PanelResults resultTbl = new Table.PanelResults();
            string stTime = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            string cmd = string.Format("Select count({0}) from {1} where {2} and {3} and {4} and {5};",
                resultTbl.ID,
                resultTbl.TableName,
                resultTbl.ModelName + "=\'" + ModelName + "\'",
                resultTbl.LoadTime + ">\'" + stTime + "\'",
                resultTbl.LoadTime + "<=\'" + endTime + "\'",
                resultTbl.ConfirmResult + "=\'FAIL\'"
                );

            object count = mCtl.ExecuteScalarCmd(mConn, cmd);
            return Convert.ToInt32(count);
        }
    }
}
