using System.Data.SQLite;
using System;
namespace SPI_AOI.DB
{
    class SQLiteConn
    {
        private static SQLiteConnection mConn;
        private static Object mLock = new Object();


        public static SQLiteConnection GetInstance()
        {
            lock (mLock)
            {
                if (mConn == null)
                {
                    mConn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", "data.db"));
                }
            }
            return mConn;
        }
    }
}