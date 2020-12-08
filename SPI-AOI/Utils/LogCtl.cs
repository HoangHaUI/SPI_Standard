using NLog;
using NLog.Targets;
using NLog.Config;
using System.Windows.Forms;
using System.IO;

namespace Heal
{
    /// <summary>
    /// Configuration Nlog by Heal 12/19/2019.
    /// <para>Save log by day.</para>
    /// <para> Show all level to console window</para>
    /// <para>Write Error, Warn, Debug, Fatal, Off, and Trace level to log file.</para>
    /// </summary>
    public class LogCtl
    {
        private static Logger Logger;
        private static LogCtl mlog;
        /// <summary>
        /// <para>en: Initialization configuration for writing to file </para>
        /// <para>vi: Cấu hình khởi tạo để ghi vào tập tin</para>
        /// </summary>
        public static void Initialization(bool Info = true, bool Debug = true, bool Warn = true, bool Error = true, bool Trace = true, bool Off = true, bool Fatal = true)
        {
            LoggingConfiguration config = new LoggingConfiguration();
            //config for console log
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget("ConsoleTarget")
            {
                Layout = "${longdate}|${level}|${message}${callsite:className=false:fileName=true:includeSourcePath=false:methodName=false}|${exception}"
            };
            config.AddTarget(consoleTarget);
            //config for file log
            FileTarget fileTarget = new FileTarget("FileTarget")
            {
                FileName = "${basedir}/logs/${date:format=yyyy_MM_dd}.log",
                Layout = "${longdate}|${level}|${message}${callsite:className=false:fileName=true:includeSourcePath=false:methodName=false}|${exception}"
            };
            config.AddTarget(fileTarget);
            // errors and warning write to file
            if(Info)
                config.AddRuleForOneLevel(LogLevel.Info, fileTarget);
            if(Error)
            config.AddRuleForOneLevel(LogLevel.Error, fileTarget); 
            if(Warn)
            config.AddRuleForOneLevel(LogLevel.Warn, fileTarget);
            if(Debug)
            config.AddRuleForOneLevel(LogLevel.Debug, fileTarget);
            if(Fatal)
            config.AddRuleForOneLevel(LogLevel.Fatal, fileTarget);
            if(Off)
            config.AddRuleForOneLevel(LogLevel.Off, fileTarget);
            if(Trace)
            config.AddRuleForOneLevel(LogLevel.Trace, fileTarget);
            // all to console
            config.AddRuleForAllLevels(consoleTarget); 
            //Activate the configuration
            LogManager.Configuration = config;
        }
        /// <summary>
        /// <para>en: Return single tone NLog object</para>
        /// <para>vi: Trả về đối tượng NLog giai điệu đơn</para>
        /// </summary>
        /// <example>
        /// <code>
        /// Logger log = MyLog.Instance;
        /// log.Error("Line Error");
        /// log.Info("Line Info");
        /// </code>
        /// </example>
        public static Logger GetInstance()
        {

                if(mlog == null)
                {
                    mlog = new LogCtl();
                    if (Logger == null)
                    {
                        Logger = LogManager.GetLogger("Debug");
                        Initialization();
                    }
                }
                return Logger;
        }
        public static void LoadLogFiles(System.Windows.Forms.ToolStripItem DropItems, int Limit = 20)
        {
            //var menuParent = menuBar.Items["Help"];
            //var dropItems = ((ToolStripMenuItem)menuParent).DropDown.Items["Logs"];
            string[] listLogFiles = Directory.GetFiles("Logs");
            Limit = listLogFiles.Length > 20 ? 20 : listLogFiles.Length;
            ToolStripMenuItem[] toolFileItem = new ToolStripMenuItem[Limit];
            for (int i = 0; i < Limit; i++)
            {
                toolFileItem[i] = new ToolStripMenuItem();
                string[] splStr = listLogFiles[listLogFiles.Length - Limit + i].Split('\\');
                toolFileItem[i].Name = splStr[splStr.Length - 1];
                toolFileItem[i].Text = splStr[splStr.Length - 1];
            }
            ((ToolStripMenuItem)DropItems).DropDown.Items.Clear();
            ((ToolStripMenuItem)DropItems).DropDown.Items.AddRange(toolFileItem);
            ((ToolStripMenuItem)DropItems).DropDown.ItemClicked += OpenLogFiles_Click;
        }
        private static void OpenLogFiles_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            string filename = string.Format("logs/{0}", e.ClickedItem.Text);
            FileInfo fi = new FileInfo(filename);
            System.Diagnostics.Process.Start(fi.FullName);
        }
    }
}
