using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using GerberLibrary;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SPI_AOI.Utils
{
    public class GerberUtils
    {
        /// <summary>
        /// return image bitmap and W-H (mm) of Metal stencil
        /// </summary>
        /// <param name="pathGerberFile"></param>
        /// <param name="dpi"></param>
        /// <param name="Foreground"></param>
        /// <param name="Background"></param>
        /// <returns></returns>
        public static GerberRenderResult Render(string pathGerberFile, float dpi, Color Foreground, Color Background)
        {
            GerberRenderResult result = new GerberRenderResult();
            var log = new StandardConsoleLog();
            GerberLibrary.Gerber.SaveIntermediateImages = false;
            GerberLibrary.Gerber.ShowProgress = false;
            GerberLibrary.Gerber.ExtremelyVerbose = false;
            GerberLibrary.Gerber.WaitForKey = false;
            GerberImageCreator.AA = false;
            if (GerberLibrary.Gerber.ThrowExceptions)
            {
                var task = Task.Run(() => GerberLibrary.Gerber.GetBitmapFromGerberFile(log, pathGerberFile, dpi, Foreground, Background));
                if (task.Wait(TimeSpan.FromSeconds(10)))
                {
                    ValueTuple<Bitmap, double, double> tempVal = task.Result;
                    Image<Gray, byte> imgGerber = new Image<Gray, byte>(tempVal.Item1);
                    
                    // add border 
                    int max = Math.Max(imgGerber.Width, imgGerber.Height) + 4;
                    int addx = (max - imgGerber.Width) / 2;
                    int addy = (max - imgGerber.Height) / 2;
                    Image<Gray, byte> imgGerberAdd = new Image<Gray, byte>(new System.Drawing.Size(imgGerber.Width + 2 *addx, imgGerber.Height + 2*addy));
                    CvInvoke.CopyMakeBorder(imgGerber, imgGerberAdd, addy, addy, addx, addx, Emgu.CV.CvEnum.BorderType.Constant, new MCvScalar(0));
                    result.GerberImage = imgGerberAdd;
                    imgGerber.Dispose();
                    imgGerber = null;
                    result.Width = tempVal.Item2;
                    result.Height = tempVal.Item3;
                    result.Status = ActionStatus.Successfully;
                }
                else
                {
                    result.Status = ActionStatus.Fail;
                }
            }
            else
            {
                var task = Task.Run(() => GerberLibrary.Gerber.GetBitmapFromGerberFile(log, pathGerberFile, dpi, Foreground, Background));
                if (task.Wait(TimeSpan.FromSeconds(10)))
                {
                    ValueTuple<Bitmap, double, double> tempVal = task.Result;
                    Image<Gray, byte> imgGerber = new Image<Gray, byte>(tempVal.Item1);
                    // add border 
                    int max = Math.Max(imgGerber.Width, imgGerber.Height) + 4;
                    int addx = (max - imgGerber.Width) / 2;
                    int addy = (max - imgGerber.Height) / 2;
                    Image<Gray, byte> imgGerberAdd = new Image<Gray, byte>(new System.Drawing.Size(imgGerber.Width + 2 * addx, imgGerber.Height + 2 * addy));
                    CvInvoke.CopyMakeBorder(imgGerber, imgGerberAdd, addy, addy, addx, addx, Emgu.CV.CvEnum.BorderType.Constant, new MCvScalar(0));
                    result.GerberImage = imgGerberAdd;
                    imgGerber.Dispose();
                    imgGerber = null;
                    result.Width = tempVal.Item2;
                    result.Height = tempVal.Item3;
                    result.Status = ActionStatus.Successfully;
                }
                else
                {
                    result.Status = ActionStatus.Fail;
                }
            }
            return result;
        }
    }
    
    public class GerberRenderResult
    {
        public ActionStatus Status { get; set; }
        public Image<Gray, byte> GerberImage { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public enum ActionStatus
    {
        Successfully,
        Fail
    }
}
