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
            InitResultTbl();
            InitImageTbl();
            InitErrorDetailsTbl();
        }
        public void InitResultTbl()
        {
            Table.Result resultTbl = new Table.Result();
            string cmd = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1});",
                resultTbl.TableName,
                resultTbl.ID + " TEXT PRIMARY KEY," +
                resultTbl.ModelName + " TEXT," +
                resultTbl.LoadTime + " TEXT," +
                resultTbl.SN + " TEXT," +
                resultTbl.RunningMode + " TEXT," +
                resultTbl.VIResult + " TEXT"
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
                imgTbl.Time + " TEXT," +
                imgTbl.Component + " TEXT," +
                imgTbl.Type + " TEXT"
                );
            mCtl.ExecuteCmd(mConn, cmd);
        }
        public string GetNewID()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }
        public int InsertNewProduct(
            string ID,
            string ModelName,
            DateTime Time,
            string SN,
            string RunningMode,
            string VIResult
            )
        {
            Table.Result resultTbl = new Table.Result();
            string cmd = string.Format("INSERT INTO {0} ({1}) values({2});",
                resultTbl.TableName,
                // --------------
                resultTbl.ID + "," +
                resultTbl.ModelName + "," +
                resultTbl.LoadTime + "," +
                resultTbl.SN + "," +
                resultTbl.RunningMode + "," +
                resultTbl.VIResult,

                //--------------
                "\'" + ID  + "\'," +
                "\'" + ModelName + "\'," +
                "\'" + Time.ToString("yyyy-MM-dd HH:mm:ss") + "\'," +
                "\'" + SN + "\'," +
                "\'" + RunningMode + "\'," +
                "\'" + VIResult + "\'"
                );
            return mCtl.ExecuteCmd(mConn, cmd);
        }
        public int InsertNewImage(
                string ID,
                DateTime TimeCapture,
                string ImagePath,
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
                resultTbl.ROI + "," +
                resultTbl.ROIGerber + "," +
                resultTbl.Type,

                //--------------
                "\'" + ID + "\'," +
                "\'" + TimeCapture.ToString("yyyy-MM-dd HH:mm:ss") + "\'," +
                "\'" + ImagePath + "\'," +
                "\'" + string.Format("{0},{1},{2},{3}",ROI.X, ROI.Y, ROI.Width, ROI.Height) + "\'," +
                "\'" + string.Format("{0},{1},{2},{3}", ROIGerber.X, ROIGerber.Y, ROIGerber.Width, ROIGerber.Height) + "\'," +
                "\'" + Type + "\'"
                );
            return mCtl.ExecuteCmd(mConn, cmd);
        }
        public int InsertNewErrorDetails(
                string ID,
                DateTime Time,
                string Component,
                string Type
            )
        {
            Table.ErrorDetails resultTbl = new Table.ErrorDetails();
            string cmd = string.Format("INSERT INTO {0} ({1}) values({2});",
                resultTbl.TableName,
                // --------------
                resultTbl.ID + "," +
                resultTbl.Time + "," +
                resultTbl.Component + "," +
                resultTbl.Type,

                //--------------
                "\'" + ID + "\'," +
                "\'" + Time.ToString("yyyy-MM-dd HH:mm:ss") + "\'," +
                "\'" + Component + "\'," +
                "\'" + Type + "\'"
                );
            return mCtl.ExecuteCmd(mConn, cmd);
        }
        public string[] GetModelName()
        {
            List<string> modelNames = new List<string>();
            Table.Result resultTbl = new Table.Result();
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
            Table.Result resultTbl = new Table.Result();
            string stTime = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            string cmd = string.Format("Select count({0}) from {1} where {2} and {3} and {4} and {5};",
                resultTbl.ID,
                resultTbl.TableName,
                resultTbl.ModelName + "=\'" + ModelName + "\'",
                resultTbl.LoadTime + ">\'" + stTime + "\'",
                resultTbl.LoadTime + "<=\'" + endTime + "\'",
                resultTbl.VIResult + "=\'PASS\'"
                );

            object count = mCtl.ExecuteScalarCmd(mConn, cmd);
            return Convert.ToInt32(count);
        }
        public int CountFail(string ModelName, DateTime StartTime, DateTime EndTime)
        {
            Table.Result resultTbl = new Table.Result();
            string stTime = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            string cmd = string.Format("Select count({0}) from {1} where {2} and {3} and {4} and {5};",
                resultTbl.ID,
                resultTbl.TableName,
                resultTbl.ModelName + "=\'" + ModelName + "\'",
                resultTbl.LoadTime + ">\'" + stTime + "\'",
                resultTbl.LoadTime + "<=\'" + endTime + "\'",
                resultTbl.VIResult + "=\'FAIL\'"
                );

            object count = mCtl.ExecuteScalarCmd(mConn, cmd);
            return Convert.ToInt32(count);
        }
    }
}
