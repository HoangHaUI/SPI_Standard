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
using Stencil.Control;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Stencil.Control.Model
{
    public class Gerber
    {
        public string ID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Rectangle ROI { get; set; }
        public double AngleRotation { get; set; }
        public FOVInfo[] FOVs { get; set; }
        public StartPoint StartPoint { get; set; }
        public Rectangle LocMark1 { get; set; }
        public Rectangle LocMark2 { get; set; }
        public byte[] GerberFile { get; set; }
        public Bitmap GerberImage { get; set; }
        public double XSearch { get; set; }
        public double YSearch { get; set; }
        public double ScoreMatching { get; set; }
        public Gerber Copy()
        {
            Gerber gerber = new Gerber();
            gerber.ID = this.ID;
            gerber.FileName = this.FileName;
            gerber.FilePath = this.FilePath;
            gerber.StartPoint = this.StartPoint;
            gerber.XSearch = this.XSearch;
            gerber.YSearch = this.YSearch;
            gerber.ScoreMatching = this.ScoreMatching;
            gerber.LocMark1 = new Rectangle(this.LocMark1.X, this.LocMark1.Y, this.LocMark1.Width, this.LocMark1.Height);
            gerber.LocMark2 = new Rectangle(this.LocMark2.X, this.LocMark2.Y, this.LocMark2.Width, this.LocMark2.Height);
            gerber.GerberFile = (byte[])this.GerberFile.Clone();
            gerber.GerberImage = new Bitmap(this.GerberImage);
            gerber.ROI = new Rectangle(this .ROI.X, this.ROI.Y, this.ROI.Width, this.ROI.Height);
            gerber.AngleRotation = this.AngleRotation;
            gerber.FOVs = new FOVInfo[FOVs.Length];
            for (int i = 0; i < FOVs.Length; i++)
            {
                gerber.FOVs[i] = new FOVInfo();
                gerber.FOVs[i].ID = FOVs[i].ID;
                gerber.FOVs[i].NO = FOVs[i].NO;
                gerber.FOVs[i].Width = FOVs[i].Width;
                gerber.FOVs[i].Height  = FOVs[i].Height;
                gerber.FOVs[i].Anchor = new System.Drawing.Point(FOVs[i].Anchor.X, FOVs[i].Anchor.Y);
            }
            return gerber;
        }
        public static Gerber GetNewGerber(string ID, string GerberPath, float Dpi, System.Drawing. Size FOV)
        {
            FileInfo fi = new FileInfo(GerberPath);
            Rectangle ROI = new Rectangle();
            StartPoint stPoint = StartPoint.TOP_LEFT;
            FOVInfo[] fovs;
            Gerber gerber = new Gerber();
            GerberRenderResult gbResult = Render(GerberPath, Dpi, System.Drawing.Color.White, Color.Black);
            if(gbResult.Status == ActionStatus.Fail)
            {
                return null;
            }
            ROI = new Rectangle(0, 0, gbResult.GerberImage.Width, gbResult.GerberImage.Height);
            using (Image<Gray, byte> img = new Image<Gray, byte>(gbResult.GerberImage))
            {
                fovs = FOVInfo.GetFOVs(ID, img, ROI, FOV, stPoint);
            }
            gerber.GerberImage = (Bitmap)gbResult.GerberImage.Clone();
            gbResult.GerberImage.Dispose();
            gbResult.GerberImage = null;
            gerber.GerberFile = File.ReadAllBytes(GerberPath);
            gerber.ID = ID;
            gerber.AngleRotation = 0;
            gerber.FileName = fi.Name;
            gerber.FilePath = fi.FullName;
            gerber.FOVs = fovs;
            gerber.ROI = ROI;
            gerber.StartPoint = stPoint;
            gerber.XSearch = 10;
            gerber.YSearch = 10;
            gerber.ScoreMatching = 0.5;
            return gerber;
        }
        public int LoadGerberImage(float dpi)
        {
            if(!Directory.Exists("TempDirs"))
            {
                Directory.CreateDirectory("TempDirs");
            }
            File.WriteAllBytes("TempDirs/" + this.FileName, this.GerberFile);
            GerberRenderResult gbResult = Render("TempDirs/" + this.FileName, dpi, Color.White, Color.Black);
            if(gbResult.Status == ActionStatus.Fail)
            {
                return -1;
            }
            this.GerberImage = (Bitmap)gbResult.GerberImage.Clone();
            gbResult.GerberImage.Dispose();
            gbResult.GerberImage = null;
            return 0;
        }
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
                    result.GerberImage = imgGerberAdd.ToBitmap();
                    imgGerberAdd.Dispose();
                    imgGerberAdd = null;
                    imgGerber.Dispose();
                    imgGerber = null;
                    result.Width = tempVal.Item2;
                    result.Height = tempVal.Item3;
                    result.Status = ActionStatus.Successfuly;
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
                    result.GerberImage = imgGerberAdd.ToBitmap();
                    imgGerberAdd.Dispose();
                    imgGerberAdd = null;
                    imgGerber.Dispose();
                    imgGerber = null;
                    result.Width = tempVal.Item2;
                    result.Height = tempVal.Item3;
                    result.Status = ActionStatus.Successfuly;
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
        public Bitmap GerberImage { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
