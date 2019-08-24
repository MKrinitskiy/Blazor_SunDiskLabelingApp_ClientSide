using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BlazorSvgHelper.Classes.SubClasses;
using CommonInterfaces;
using Geometry;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class BPaintEllipse : BPaintObject
    {
        private BPaintVertex _pt2 = null;
        private BPaintVertex _pt3 = null;

        public BPaintEllipse()
        {
            ObjectType = BPaintOpbjectType.Ellipse;
            MandatoryVerticesCount = 3;
        }

        public BPaintVertex pt2
        {
            get { return _pt2; }
            set
            {
                _pt2 = value;
                VerticesList.Add(_pt2);
            }
        }

        public BPaintVertex pt3
        {
            get { return _pt3; }
            set
            {
                _pt3 = value;
                VerticesList.Add(_pt3);
            }
        }



        public override string ToString()
        {
            return "BPaintEllipse, ID=" + ObjectID;
        }


        public PointD centerPointD
        {
            get
            {
                if (VerticesList.Count < 2)
                {
                    return Position.PtD;
                }
                else
                {
                    return (Position.PtD + new SizeD(pt2.x, pt2.y))/2.0d;
                }
            }
        }

        public double axisInclinationRad
        {
            get
            {
                if (VerticesList.Count < 2)
                {
                    return double.NaN;
                }
                else
                {
                    //double alpha_sign = 1.0;
                    double alpha_rad = 0.0;
                    double dx = pt2.x - Position.x;
                    double dy = pt2.y - Position.y;
                    if (dx == 0.0)
                    {
                        alpha_rad = Math.PI / 2.0;
                    }
                    else
                    {
                        double tg_alpha = dy / dx;
                        alpha_rad = Math.Atan(tg_alpha);
                    }

                    return alpha_rad;
                }
            }
        }


        public double aRadius
        {
            get
            {
                if (VerticesList.Count < 2)
                {
                    return double.NaN;
                }
                else
                {
                    double x_diameter = pt2.PtD.DistanceTo(Position.PtD);
                    double a_axis_radius = x_diameter * 0.5;
                    return a_axis_radius;
                }
            }
        }



        public double bRadius
        {
            get
            {
                if (VerticesList.Count < 2)
                {
                    return double.NaN;
                }
                else
                {
                    double xCenter = centerPointD.X;
                    double yCenter = centerPointD.Y;
                    PointD pt3_centered = pt3.PtD - new SizeD(xCenter, yCenter);
                    DenseVector pt3_centered_coords = DenseVector.OfArray(new double[] { pt3_centered.X, pt3_centered.Y });
                    DenseMatrix rotation_matrix = DenseMatrix.OfArray(new double[,]
                    {
                        {Math.Cos(axisInclinationRad), Math.Sin(axisInclinationRad)},
                        {-Math.Sin(axisInclinationRad), Math.Cos(axisInclinationRad)}
                    });
                    DenseVector pt3_centered_rotated_inv_coords = rotation_matrix.Multiply(pt3_centered_coords) as DenseVector;
                    PointD pt3_centered_rotated_inv = new PointD(pt3_centered_rotated_inv_coords[0], pt3_centered_rotated_inv_coords[1]);

                    double b_axis_radius = 0.0;
                    if (aRadius == 0.0)
                    {
                        b_axis_radius = Math.Abs(pt3_centered_rotated_inv.Y);
                    }
                    else
                    {
                        if (4 * (aRadius * aRadius) - 4 * (pt3_centered_rotated_inv.X * pt3_centered_rotated_inv.X) - pt3_centered_rotated_inv.Y * pt3_centered_rotated_inv.Y <= 0.0)
                        {
                            b_axis_radius = Math.Abs(pt3_centered_rotated_inv.Y);
                        }
                        else
                        {
                            b_axis_radius = Math.Abs(pt3_centered_rotated_inv.Y) / Math.Sqrt(1.0 - (pt3_centered_rotated_inv.X * pt3_centered_rotated_inv.X) / (aRadius * aRadius));
                        }
                    }

                    return b_axis_radius;
                }
            }
        }



        public SizeD boundingRectangleSizeD
        {
            get
            {
                if (VerticesList.Count < 3)
                {
                    return SizeD.Empty;
                }
                else
                {
                    double dx = 2 * Math.Sqrt(Math.Pow(aRadius*Math.Cos(axisInclinationRad), 2) + Math.Pow(bRadius * Math.Sin(axisInclinationRad), 2));
                    double dy = 2 * Math.Sqrt(Math.Pow(aRadius * Math.Sin(axisInclinationRad), 2) + Math.Pow(bRadius * Math.Cos(axisInclinationRad), 2));
                    return new SizeD(dx, dy);
                }
            }
        }



        public ellipse SvgEllipseDescription()
        {
            if (VerticesList.Count < 3)
            {
                return null;
            }

            transform = new SVGtransform(rotateCx: centerPointD.X, rotateCy: centerPointD.Y,
                rotateAngleDegrees: (axisInclinationRad / Math.PI) * 180.0);

            ellipse retEllipse = new ellipse()
            {
                cx = centerPointD.X,
                cy = centerPointD.Y,
                rx = aRadius,
                ry = bRadius,
                fill = "none",
                stroke = Color,
                stroke_width = 2,
                transform = transform.ToString(),
            };
            return retEllipse;
        }


        public bool IsValid()
        {
            if (VerticesList.Count < 3) return false;
            return (!((Position.Equals(pt2)) & (Position.Equals(pt3))));
        }

        
    }
}
