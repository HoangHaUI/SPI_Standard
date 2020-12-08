using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Collections;
using System.Drawing;

namespace SPI_AOI.Models
{
    public class ShowModel
    {
        private static void RemovePad(Model model)
        {
            for (int i = 0; i < model.Gerber.RemoveROI.Count; i++)
            {
                CvInvoke.Rectangle(model.ImgGerberProcessed, model.Gerber.RemoveROI[i], new MCvScalar(0), -1);
            }
        }
        private static void DrawColor(Model model)
        {
            if (model.ImgGerberProcessedBgr == null || model.ImgGerberProcessedBgr.Size != model.ImgGerberProcessed.Size)
            {
                model.ImgGerberProcessedBgr = new Image<Bgr, byte>(model.ImgGerberProcessed.Size);
            }
            CvInvoke.CvtColor(model.ImgGerberProcessed, model.ImgGerberProcessedBgr, Emgu.CV.CvEnum.ColorConversion.Gray2Bgr);
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                model.ImgGerberProcessed.ROI = model.Gerber.ROI;
                model.ImgGerberProcessedBgr.ROI = model.Gerber.ROI;
                CvInvoke.FindContours(model.ImgGerberProcessed, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                CvInvoke.DrawContours(model.ImgGerberProcessedBgr, contours, -1, new MCvScalar(model.Gerber.Color.B, model.Gerber.Color.G, model.Gerber.Color.R), -1);
                model.ImgGerberProcessedBgr.ROI = Rectangle.Empty;
                model.ImgGerberProcessed.ROI = Rectangle.Empty;
            }
        }
        public static void GetImage(Model model)
        {
            if (model.ImgGerberProcessed != null)
            {
                model.ImgGerberProcessed.Dispose();
                model.ImgGerberProcessed = null;
            }
            model.ImgGerberProcessed = model.Gerber.ProcessingGerberImage.Copy();
        }
        private static void HightLightSelectPad(Image<Gray, byte> ImgGray, Image<Bgr, byte> ImgDraw , Rectangle SelectRect)
        {
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(ImgGray, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    Rectangle bound = CvInvoke.BoundingRectangle(contours[i]);
                    if(SelectRect.Contains(bound))
                    {
                        CvInvoke.DrawContours(ImgDraw, contours, i, new MCvScalar(255, 255, 255), -1);
                    }
                }
            }
        }
        public static Image<Bgr, byte> GetLayoutImage(Model model , ActionMode mode)
        {
            Image<Bgr, byte> img = null;


            if (model.Gerber is GerberFile)
            {
                if (model.Gerber.Visible)
                {
                    switch (mode)
                    {
                        case ActionMode.Render:
                            GetImage(model);
                            RemovePad(model);
                            DrawColor(model);
                            break;
                        case ActionMode.Rotation:
                            GetImage(model);
                            RemovePad(model);
                            DrawColor(model);
                            break;
                        case ActionMode.Update_Color_Gerber:
                            DrawColor(model);
                            break;
                        case ActionMode.Draw_Cad:
                            break;
                        case ActionMode.Select_Pad:
                            break;
                        default:
                            break;
                    }
                }
                if (model.Gerber.Visible)
                {
                    img = model.ImgGerberProcessedBgr.Copy();
                    if (model.Gerber.SelectPad != Rectangle.Empty)
                    {
                        HightLightSelectPad(model.ImgGerberProcessed, img, model.Gerber.SelectPad);
                    }
                }
                else
                {
                    img = new Image<Bgr, byte>(model.ImgGerberProcessedBgr.Size);
                }
                foreach (CadFile item in model.Cad)
                {
                    if (item.Visible)
                    {
                        int x = item.X;
                        int y = item.Y;
                        double angle = item.Angle * Math.PI / 180.0;
                        Color cl = item.Color;
                        foreach (CadItem caditem in item.CadItems)
                        {
                            Point ct = Point.Round(caditem.Center);
                            string txt = caditem.Name;
                            ct.X += x;
                            ct.Y += y;
                            Point newCtRotate = ImageProcessingUtils.PointRotation(ct, new Point(item.CenterRotation.X + item.X, item.CenterRotation.Y + item.Y), angle);
                            MCvScalar color = new MCvScalar(cl.B, cl.G, cl.R);
                            if (item.SelectCenter != Rectangle.Empty)
                            {
                                Rectangle bound = new Rectangle(newCtRotate.X, newCtRotate.Y, 1, 1);
                                if (item.SelectCenter.Contains(bound))
                                {
                                    color = new MCvScalar(255, 255, 255);
                                }
                            }
                            if(model.ShowComponentCenter)
                            {
                                CvInvoke.Circle(img, newCtRotate, 3, color, -1);
                            }
                            if(model.ShowComponentName)
                            {
                                newCtRotate.X += 5;
                                CvInvoke.PutText(img, txt, newCtRotate, Emgu.CV.CvEnum.FontFace.HersheyDuplex, 0.9, color, 1);
                            }
                            
                        }
                    }
                }
            }

            return img;
        }
        public Image<Gray, byte> RotateImage(Image<Gray, byte> ImgInput, double Angle)
        {
            return ImageProcessingUtils.ImageRotation(ImgInput, new Point(ImgInput.Width / 2, ImgInput.Height / 2), Angle);
        }
    }
    public enum ActionMode
    {
        Render,
        Update_Color_Gerber,
        Draw_Cad,
        Select_Pad,
        Rotation,
    }
}
