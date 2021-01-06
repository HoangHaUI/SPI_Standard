using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;


namespace SPI_AOI.Models
{
    public class Fov
    {
        public string ID { get; set; }
        public int NO { get; set; }
        public Point Anchor { get; set; }
        public Rectangle ROI { get; set; }
        public List<int> PadItems { get; set; }
        private static Properties.Settings mParam = Properties.Settings.Default;
        public static List<Fov> GetFov(string ID, Image<Gray, byte> ImgGerber, Rectangle ROI, Size FOV,  SPI_AOI.Utils.StartPoint StartPoint)
        {
            List<Fov> fovs = new List<Fov>();
            Point[] anchors = SPI_AOI.Utils.FOVOptimize.GetAnchorsFOV(ImgGerber, ROI, FOV, StartPoint);
            for (int i = 0; i < anchors.Length; i++)
            {
                Fov fov = new Fov();
                fov.ID = ID;
                fov.NO = i;
                fov.Anchor = anchors[i];
                fov.ROI = new Rectangle(mParam.IMAGE_SIZE.Width /2 - FOV.Width / 2, mParam.IMAGE_SIZE.Height/2 - FOV.Height / 2, FOV.Width, FOV.Height);
                fov.PadItems = new List<int>();
                fovs.Add(fov);
            }
            return fovs;
        }
        public static Image<Bgr, byte> GetFOVDiagram(Image<Gray, byte> ImgGerber, Point[] Anchor, Size FOV, int IndexHightLight)
        {
            return SPI_AOI.Utils.FOVOptimize.GetDiagramHightLight(ImgGerber, Anchor, FOV, IndexHightLight);
        }
    }
}
