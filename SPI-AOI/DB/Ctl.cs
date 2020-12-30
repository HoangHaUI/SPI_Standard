using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SQLite;
using NLog;
using System.Drawing;
using Heal;
using System.Collections;

namespace SPI_AOI.DB
{
    class GenCtl
    {
        public SQLiteConnection mConn = SQLiteConn.GetInstance();
        private Logger mLog = Heal.LogCtl.GetInstance();
        public int ExecuteCmd(SQLiteConnection Conn, string cmd)
        {
            int result = 0;
            Thread.Sleep(5);
            lock (Conn)
            {
                Conn.Open();
                using (SQLiteCommand command = Conn.CreateCommand())
                {
                    try
                    {
                        command.CommandText = cmd;
                        result = command.ExecuteNonQuery();
                    }
                    catch (SQLiteException ex)
                    {
                        mLog.Error(ex.Message);
                        result = -2;
                    }
                }
                Conn.Close();
            }
            return result;
        }
        public object ExecuteScalarCmd(SQLiteConnection Conn, string cmd)
        {
            object result = null;
            Thread.Sleep(5);
            lock (Conn)
            {
                Conn.Open();
                using (SQLiteCommand command = Conn.CreateCommand())
                {
                    try
                    {
                        command.CommandText = cmd;
                        result = command.ExecuteScalar();
                    }
                    catch (SQLiteException ex)
                    {
                        mLog.Error(ex.Message);
                    }
                }
                Conn.Close();
            }
            return result;
        }
        public ArrayList ExecuteReader(SQLiteConnection Conn, string cmd)
        {
            ArrayList result = new ArrayList();
            Thread.Sleep(5);
            lock (Conn)
            {
                Conn.Open();
                using (SQLiteCommand command = Conn.CreateCommand())
                {
                    try
                    {
                        command.CommandText = cmd;
                        SQLiteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Dictionary<string, object> obj = new Dictionary<string, object>();
                            for (int i = 0; i < reader.VisibleFieldCount; i++)
                            {
                                obj.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            result.Add(obj);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        mLog.Error(ex.Message);
                    }
                }
                Conn.Close();
            }
            return result;
        }
    }
}