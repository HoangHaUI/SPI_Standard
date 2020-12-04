using GerberLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Stencil.SDK
{
    public class GerberToBitmap
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
            Gerber.SaveIntermediateImages = false;
            Gerber.ShowProgress = false;
            Gerber.ExtremelyVerbose = false;
            Gerber.WaitForKey = false;
            Gerber.ShowProgress = true;
            GerberImageCreator.AA = false;
            if (Gerber.ThrowExceptions)
            {
                var task = Task.Run(() => Gerber.GetBitmapFromGerberFile(log, pathGerberFile, dpi, Foreground, Background));
                if (task.Wait(TimeSpan.FromSeconds(10)))
                {
                    ValueTuple<Bitmap, double, double> tempVal = task.Result;
                    result.GerberImage = tempVal.Item1;
                    result.Width = tempVal.Item2;
                    result.Height = tempVal.Item3;
                    result.Status = GerberDecodeStatus.Successfuly;
                }
                else
                {
                    result.Status = GerberDecodeStatus.Fail;
                }
            }
            else
            {
                var task = Task.Run(() => Gerber.GetBitmapFromGerberFile(log, pathGerberFile, dpi, Foreground, Background));
                if (task.Wait(TimeSpan.FromSeconds(10)))
                {
                    ValueTuple<Bitmap, double, double> tempVal = task.Result;
                    result.GerberImage = tempVal.Item1;
                    result.Width = tempVal.Item2;
                    result.Height = tempVal.Item3;
                    result.Status = GerberDecodeStatus.Successfuly;
                }
                else
                {
                    result.Status = GerberDecodeStatus.Fail;
                }
                    
                
            }
            return result;
        }
    }
    public class GerberRenderResult
    {
        public GerberDecodeStatus Status { get; set; }
        public Bitmap GerberImage { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public enum GerberDecodeStatus
    {
        Successfuly,
        Fail,
    }
}
