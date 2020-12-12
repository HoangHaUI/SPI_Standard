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
using System.Threading;
using System.Diagnostics;

namespace SPI_AOI.Models
{
    public class ShowModel
    {
        //private static void RemovePad(Model model)
        //{
        //    for (int i = 0; i < model.Gerber.RemoveROI.Count; i++)
        //    {
        //        CvInvoke.Rectangle(model.ImgGerberProcessed, model.Gerber.RemoveROI[i], new MCvScalar(0), -1);
        //    }
        //}
        private static void DrawColor(Model model)
        {
            model.ImgGerberProcessedBgr = new Image<Bgr, byte>(model.Gerber.ProcessingGerberImage.Size);
            Color cl = model.Gerber.Color;
            using (VectorOfVectorOfPoint contoursLinked = new VectorOfVectorOfPoint())
            using (VectorOfVectorOfPoint contoursNoneLink = new VectorOfVectorOfPoint())
            {
                for (int i = 0; i < model.Gerber.PadItems.Count; i++)
                {
                    var item = model.Gerber.PadItems[i];
                    if (!string.IsNullOrEmpty(item.CadFileID) && item.CadItemIndex != -1)
                    {
                        contoursLinked.Push(item.Contour);
                    }
                    else
                    {
                            contoursNoneLink.Push(item.Contour);
                    }
                }
                if (model.HightLineLinkedPad)
                {
                    CvInvoke.DrawContours(model.ImgGerberProcessedBgr, contoursLinked, -1, new MCvScalar(255, 0, 0), -1);
                }
                else
                {
                    CvInvoke.DrawContours(model.ImgGerberProcessedBgr, contoursLinked, -1, new MCvScalar(cl.B, cl.G, cl.R), -1);
                }
                CvInvoke.DrawContours(model.ImgGerberProcessedBgr, contoursNoneLink, -1, new MCvScalar(cl.B, cl.G, cl.R), -1);
            }

        }
        private static void HightLightSelectPad(Image<Bgr, byte> ImgDraw , Model model)
        {
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                for (int i = 0; i < model.Gerber.PadItems.Count; i++)
                {
                    Rectangle bound = CvInvoke.BoundingRectangle(model.Gerber.PadItems[i].Contour);

                    if (model.Gerber.SelectPad.Contains(bound))
                    {
                        contours.Push(model.Gerber.PadItems[i].Contour);
                    }
                }
                CvInvoke.DrawContours(ImgDraw, contours, -1, new MCvScalar(255, 255, 255), -1);
            }
        }
        private static void HightLightMarkPoint(Image<Bgr, byte> ImgDraw, Model model)
        {
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                for (int i = 0; i < 2; i++)
                {
                    int idMark = model.Gerber.MarkPoint.PadMark[i];
                    if (idMark != -1)
                    {
                        contours.Push(model.Gerber.PadItems[idMark].Contour);
                        Point ct = model.Gerber.PadItems[idMark].Center;
                        CvInvoke.PutText(ImgDraw, "G" + (i + 1).ToString() , new Point(ct.X + 10, ct.Y - 10), Emgu.CV.CvEnum.FontFace.HersheyDuplex, 0.9, new MCvScalar(0, 255, 255), 1, Emgu.CV.CvEnum.LineType.Filled);
                    }
                }
                CvInvoke.DrawContours(ImgDraw, contours, -1, new MCvScalar(0, 255, 255), -1);
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
                            DrawColor(model);
                            break;
                        case ActionMode.Rotation:
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
                        HightLightSelectPad(img, model);
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
                            if (model.ShowLinkLine)
                            {
                                if(caditem.Name != "UNDEFINE")
                                {
                                    for (int i = 0; i < caditem.PadsIndex.Count; i++)
                                    {
                                        CvInvoke.Line(img, newCtRotate, model.Gerber.PadItems[caditem.PadsIndex[i]].Center, new MCvScalar(0, 255, 0), 1);
                                    }
                                }
                                
                            }
                            if (model.ShowComponentName)
                            {
                                newCtRotate.X += 5;
                                CvInvoke.PutText(img, txt, newCtRotate, Emgu.CV.CvEnum.FontFace.HersheyDuplex, 0.5, color, 1, Emgu.CV.CvEnum.LineType.Filled);
                            }
                        }
                        
                    }
                }
                if (model.Gerber.Visible)
                {
                    HightLightMarkPoint(img, model);
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
