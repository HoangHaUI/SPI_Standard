using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;


namespace SPI_AOI.Models
{
    public class Mark
    {
        public string GerberID { get; set; }
        public List<int> PadMark { get; set; }
        public double SearchX { get; set; }
        public double SearchY { get; set; }
        public double Score { get; set; }
        public double ThresholdValue { get; set; }

        public Mark() { }
        public Mark(string GerberID, double SearchX = 5, double SearchY = 5, double Score = 50)
        {
            this.GerberID = GerberID;
            this.PadMark = new List<int>();
            PadMark.Add(-1);
            PadMark.Add(-1);
            this.SearchX = SearchX;
            this.SearchY = SearchY;
            this.Score = Score;
            this.ThresholdValue = 127;
        }
        public static Tuple<VectorOfPoint, double> MarkDetection(Image<Gray, byte> ImgBinary, VectorOfPoint Template)
        {
            VectorOfPoint mark = null;
            double crScore = 1;
            double areaTemplate = CvInvoke.ContourArea(Template);
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(ImgBinary, contours, null, Emgu.CV.CvEnum.RetrType.Ccomp, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    double scoreMatching = CvInvoke.MatchShapes(Template, contours[i], Emgu.CV.CvEnum.ContoursMatchType.I3);
                    double scoreCurrent = CvInvoke.ContourArea(contours[i]);
                    double scoreArea = Math.Min(areaTemplate, scoreCurrent) / Math.Max(areaTemplate, scoreCurrent);
                    scoreArea = 1 - scoreArea;
                    double score = Math.Max(scoreMatching, scoreArea);
                    if (score < crScore)
                    {
                        if(mark != null)
                        {
                            mark.Dispose();
                            mark = null;
                        }
                        crScore = score;
                        mark = new VectorOfPoint(contours[i].ToArray());
                    }
                }
            }
            return new Tuple<VectorOfPoint, double>(mark, crScore);
        }
    }
}
