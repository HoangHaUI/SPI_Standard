using System;
using System.IO;
using NLog;

namespace Heal
{
    class FilesManagement
    {
        private static Logger mLog = LogCtl.GetInstance();
        public static void DeleteFiles(string path, int hours, bool subfolder = false, bool WriteLog = true)
        {
            if(Directory.Exists(path))
            {
                string[] subPath = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                int count = 0;
                foreach (string item in files)
                {
                    FileInfo fi = new FileInfo(item);
                    if (fi.LastWriteTime.AddHours(hours) < DateTime.Now)
                    {
                        try
                        {
                            fi.Delete();
                            count++;
                            mLog.Info(string.Format("[Cleaner] Remove {0} complete!", fi));
                        }
                        catch (Exception e)
                        {
                            mLog.Warn(e.Message);
                        }
                    }
                }
                if (count > 0 && WriteLog)
                {
                    mLog.Info(string.Format("[Cleaner] Remove {0}/{1} files at {2} complete!", count, files.Length, path));
                }
                foreach (string item in subPath)
                {
                    DeleteFiles(item, hours, subfolder: true, WriteLog:WriteLog);
                }
                if (subfolder)
                {
                    if (files == null || files.Length == count)
                    {
                        Directory.Delete(path);
                    }
                }
            }
        }
    }
}
