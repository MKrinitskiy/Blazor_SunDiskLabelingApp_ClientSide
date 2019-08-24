using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommonInterfaces;
//using Emgu.CV.Structure;

namespace Geometry
{
    /// <summary>
    /// Class RoundData.
    /// Хранит и рассчитывает различные параметры окружности на изобржении
    /// </summary>
    [Serializable]
    public class RoundData : CsvExportable
    {
        public double RoundArea = 0.0;
        private bool nullCircle;

        private int intCenterX;

        [XmlElement("intCenterX")]
        public int IntCenterX
        {
            get { return intCenterX; }
            set
            {
                intCenterX = value;
                dCenterX = (double)intCenterX;
            }
        }

        private int intCenterY;

        [XmlElement("intCenterY")]
        public int IntCenterY
        {
            get { return intCenterY; }
            set
            {
                intCenterY = value;
                dCenterY = (double)intCenterY;
            }
        }

        private double dCenterX;

        [XmlElement("doubleCenterX")]
        public double DCenterX
        {
            get { return dCenterX; }
            set
            {
                dCenterX = value;
                intCenterX = Convert.ToInt32(Math.Round(dCenterX, 0));
            }
        }

        private double dCenterY;

        [XmlElement("doubleCenterY")]
        public double DCenterY
        {
            get { return dCenterY; }
            set
            {
                dCenterY = value;
                intCenterY = Convert.ToInt32(Math.Round(dCenterY, 0));
            }
        }

        private double dRadius;

        [XmlElement("doubleRadius")]
        public double DRadius
        {
            get { return dRadius; }
            set
            {
                dRadius = value;
                RoundArea = Math.PI * dRadius * dRadius;
            }
        }

        public PointF pointfCircleCenter()
        {
            return (new PointF((float)dCenterX, (float)dCenterY));
        }


        public PointD pointDCircleCenter()
        {
            return (new PointD(dCenterX, dCenterY));
        }


        public RoundData()
        {
            DCenterX = 0.0d;
            DCenterY = 0.0d;
            DRadius = 0.0d;
        }


        public RoundData(double centerX, double centerY, double radius)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius))
            {
                nullCircle = true;
            }
            else
            {
                DCenterX = centerX;
                DCenterY = centerY;
                DRadius = radius;
            }
        }



        public RoundData Copy()
        {
            if (nullCircle)
            {
                return RoundData.nullRoundData();
            }
            else
            {
                return new RoundData(DCenterX, DCenterY, DRadius);
            }
        }



        public override string ToString()
        {
            if (!IsNull)
            {
                string str = "center point: ( " + dCenterX.ToString() + " , " + dCenterY.ToString() + " );" +
                             Environment.NewLine;
                str += "radius: " + dRadius.ToString();
                return str;
            }
            else return "N/A";
        }




        public string ToCSVstring(string separator=";")
        {
            if (!IsNull)
            {
                return "" + dCenterX + separator + dCenterY + separator + dRadius;
            }
            else return "N/A";
        }




        public bool IsNull
        {
            get { return nullCircle; }
            set
            {
                this.nullCircle = value;
                if (nullCircle)
                {
                    this.DCenterX = 0.0d;
                    this.DCenterY = 0.0d;
                    this.DRadius = 0.0d;
                }
            }
        }

        public static RoundData nullRoundData()
        {
            RoundData rd = new RoundData();
            rd.IsNull = true;
            return rd;
        }



        public static bool isNull(RoundData rd)
        {
            return rd.IsNull;
        }


        //public CircleF CircleF()
        //{
        //    CircleF c = new CircleF(pointfCircleCenter(), (float)DRadius);
        //    return c;
        //}
    }


    [Serializable]
    public class RoundDataWithUnderlyingImgSize : CsvExportable
    {
        public RoundData circle { get; set; }
        public Size imgSize { get; set; }

        public RoundDataWithUnderlyingImgSize() { }

        public RoundDataWithUnderlyingImgSize(RoundData sunDiskRoundData, Size underlyingImageSize)
        {
            circle = sunDiskRoundData;
            imgSize = underlyingImageSize;
        }
    }
}
