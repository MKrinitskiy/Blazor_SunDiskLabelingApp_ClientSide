using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Geometry
{
    public class LineDescription2D
    {
        public PointD p0 = new PointD();
        public Vector2D directionVector = new Vector2D();


        public double directionVectorLength
        {
            get { return directionVector.VectorLength; }
        }


        public LineDescription2D()
        {
            p0 = new PointD(0.0d, 0.0d);
            directionVector = new Vector2D()
            {
                X = 1.0d,
                Y = 1.0d
            };
        }


        public LineDescription2D(PointD _p0, Vector2D _direction)
        {
            p0 = _p0;
            directionVector = _direction;
            directionVector.VectorLength = 1.0d;
        }




        public static PointD CrossPoint(LineDescription2D l1, LineDescription2D l2)
        {
            PointD pc = new PointD();

            if ((l1.directionVector == l2.directionVector) || (l1.directionVector == -l2.directionVector))
            {
                return PointD.nullPointD();
            }

            double d1 = l2.p0.Y * l1.directionVector.X - l1.p0.Y * l1.directionVector.X + l1.p0.X * l1.directionVector.Y -
                        l2.p0.X * l1.directionVector.Y;
            double d2 = l1.directionVector.Y * l2.directionVector.X - l1.directionVector.X * l2.directionVector.Y;
            double t2Cross = d1 / d2;
            pc = (l2.p0 + l2.directionVector * t2Cross).ToPointD();

            return pc;
        }




        public double Distance(PointD pt)
        {
            Vector2D ptVec = new Vector2D(p0, pt);
            double proj = Math.Abs(directionVector * ptVec);
            return Math.Sqrt(ptVec.VectorLength * ptVec.VectorLength - proj * proj);
        }
    }





    public class LineDescription3D
    {
        public Point3D p0 = new Point3D();
        public Vector3D directionVector = new Vector3D();


        public double directionVectorLength
        {
            get { return directionVector.VectorLength; }
        }


        public LineDescription3D()
        {

        }


        public LineDescription3D(Point3D _p0, Vector3D _direction)
        {
            p0 = _p0;
            directionVector = new Vector3D(_direction);
            directionVector.VectorLength = 1.0d;
        }



        public double Distance(Point3D pt)
        {
            Vector3D ptVec = new Vector3D(p0, pt);
            double proj = Math.Abs(directionVector * ptVec);
            return Math.Sqrt(ptVec.VectorLength * ptVec.VectorLength - proj * proj);
        }





        public static Point3D CrossPoint(LineDescription3D l1, LineDescription3D l2)
        {
            DenseMatrix dmFactorsA = DenseMatrix.Create(2, 2, (r, c) =>
            {
                if (r == 0) // x row
                {
                    switch (c)
                    {
                        case 0:
                            return l1.directionVector.X;
                            break;
                        case 1:
                            return -l2.directionVector.X;
                            break;
                        default:
                            break;
                    }
                }
                if (r == 1) // y row
                {
                    switch (c)
                    {
                        case 0:
                            return l1.directionVector.Y;
                            break;
                        case 1:
                            return -l2.directionVector.Y;
                            break;
                        default:
                            break;
                    }
                }
                return 0.0d;
            });


            DenseVector dvFactorB = DenseVector.Create(2, r =>
            {
                switch (r)
                {
                    case 0: // x row
                        return l2.p0.X - l1.p0.X;
                        break;
                    case 1: // y row
                        return l2.p0.Y - l1.p0.Y;
                        break;
                    default:
                        break;
                }
                return 0.0d;
            });

            DenseVector dvKvaluesResult = (DenseVector)dmFactorsA.LU().Solve(dvFactorB);
            Point3D cross1 = l1.p0 + dvKvaluesResult[0]*l1.directionVector;
            Point3D cross2 = l2.p0 + dvKvaluesResult[1]*l2.directionVector;
            if (cross1 == cross2)
            {
                return cross1;
            }
            else return Point3D.nullPoint3D();
        }
    }






    public class Plane3D
    {
        private Point3D p0 = new Point3D();
        private Vector3D n = new Vector3D(1.0d, 1.0d, 1.0d);

        private bool nullPlane = false;


        public bool IsNull
        {
            get { return nullPlane; }
            set
            {
                this.nullPlane = value;
                this.p0 = Point3D.nullPoint3D();
                this.n = new Vector3D(0.0d, 0.0d, 0.0d);
            }
        }



        public static Plane3D nullPlane3D()
        {
            Plane3D plane = new Plane3D();
            plane.IsNull = true;
            return plane;
        }


        public Plane3D()
        {
            n.VectorLength = 1.0d;
        }


        public Plane3D(Point3D _p0, Vector3D _n)
        {
            p0 = _p0;
            n = _n;
            n.VectorLength = 1.0d;
        }



        public Plane3D(LineDescription3D l1, LineDescription3D l2)
        {
            Point3D ptLinesCross = LineDescription3D.CrossPoint(l1, l2);
            if (ptLinesCross.IsNull)
            {
                return;
            }
            else
            {
                p0 = ptLinesCross;
                n = l1.directionVector.VectorProduct(l2.directionVector);
                n.VectorLength = 1.0d;
            }
        }



        public double Distance(Point3D pt)
        {
            Vector3D ptVec = new Vector3D(p0, pt);
            double nProj = Math.Abs(n * ptVec);
            return nProj;
        }



        public Point3D PointAtPlane(PointD ptd)
        {
            Point3D retPt3D = new Point3D(ptd.X, ptd.Y, 0.0d);

            PointD ptd0 = new PointD(p0.X, p0.Y);
            Vector2D vXY = new Vector2D(ptd0, ptd);
            Vector2D nXY = new Vector2D(n.X, n.Y);
            Vector2D lXY = new Vector2D(n.Y, -n.X);

            if (n.Z == 0.0d)
            {
                //return retPt3D;
                // плоскость параллельна оси Z - значит,
                // либо надо вернуть пустую точку
                // либо любую, лежащую в плоскости.
                if ((new LineDescription2D(ptd0, lXY)).Distance(ptd) == 0.0d)
                {
                    // точка лежит на прямой пересечения исследуемой плоскости с плоскостью OXY
                    // значит, принадлежит этой плоскости
                    // значит, ее же можно и вернуть - просто Z приравнять к чему угодно. Например, 0.0d
                    // а это уже сделано по умолчанию
                    return retPt3D;
                }
                else
                {
                    return Point3D.nullPoint3D();
                }
            }

            retPt3D.Z = p0.Z - (vXY*nXY)/n.Z;
            return retPt3D;
        }
    }





    public class Vector2D // : IEnumerable<double>
    {
        private double x = 0.0d;
        private double y = 0.0d;


        public Vector2D()
        {
        }


        public Vector2D(double _x, double _y)
        {
            X = _x;
            Y = _y;
        }



        public Vector2D(IEnumerable<double> v)
        {
            X = v.ElementAt(0);
            Y = v.ElementAt(1);
        }



        public Vector2D(PointD p0, PointD p)
        {
            x = p.X - p0.X;
            y = p.Y - p0.Y;
        }



        public Vector2D(PointD p)
        {
            x = p.X;
            y = p.Y;
        }


        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }


        public Vector2D Add(Vector2D v2)
        {
            Vector2D retV = new Vector2D();
            retV.X = this.X + v2.X;
            retV.Y = this.Y + v2.Y;
            return retV;
        }



        public Vector2D Add(IEnumerable<double> v2)
        {
            Vector2D retV = new Vector2D();
            retV.X = this.X + v2.ElementAt(0);
            retV.Y = this.Y + v2.ElementAt(1);
            return retV;
        }



        public Vector2D Subtract(Vector2D v2)
        {
            Vector2D retV = new Vector2D();
            retV.X = this.X - v2.X;
            retV.Y = this.Y - v2.Y;
            return retV;
        }



        public Vector2D Subtract(IEnumerable<double> v2)
        {
            Vector2D retV = new Vector2D();
            retV.X = this.X - v2.ElementAt(0);
            retV.Y = this.Y - v2.ElementAt(1);
            return retV;
        }



        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return v1.Add(v2);
        }




        public static Vector2D operator +(Vector2D v1, PointD ptd)
        {
            return v1.Add(new Vector2D(ptd.X, ptd.Y));
        }



        public static Vector2D operator +(PointD ptd, Vector2D v1)
        {
            return v1.Add(new Vector2D(ptd.X, ptd.Y));
        }



        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return v1.Subtract(v2);
        }



        public static Vector2D operator -(Vector2D v1)
        {
            return (new Vector2D()).Subtract(v1);
        }



        public double ScalarProduct(Vector2D v2)
        {
            return (X * v2.X + Y * v2.Y);
        }


        public Vector2D Product(double d)
        {
            return new Vector2D(X * d, Y * d);
        }


        public static double operator *(Vector2D v1, Vector2D v2)
        {
            return v1.ScalarProduct(v2);
        }


        public static Vector2D operator *(Vector2D v1, double d)
        {
            return v1.Product(d);
        }



        public static Vector2D operator *(double d, Vector2D v1)
        {
            return v1.Product(d);
        }



        public double VectorLength
        {
            get { return Math.Sqrt(x * x + y * y); }
            set
            {
                if (value == VectorLength)
                {
                    return;
                }
                else if (VectorLength == 0.0d)
                {
                    Exception ex = new Exception("zero-length vector length cant be modified");
                    throw ex;
                }
                else
                {
                    double k = value / VectorLength;
                    x *= k;
                    y *= k;
                }
            }
        }



        public PointD ToPointD()
        {
            return new PointD(X, Y);
        }


        //public IEnumerator<double> GetEnumerator()
        //{
        //    return (IEnumerator<double>)(new double[2] { x, y }.GetEnumerator());
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }






    public class Vector3D : IEnumerable<double>
    {
        private double x = 0.0d;
        private double y = 0.0d;
        private double z = 0.0d;


        public Vector3D()
        {
        }


        public Vector3D(double _x, double _y, double _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }



        public Vector3D(IEnumerable<double> v)
        {
            X = v.ElementAt(0);
            Y = v.ElementAt(1);
            Z = v.ElementAt(2);
        }



        public Vector3D(Point3D p0, Point3D p)
        {
            x = p.X - p0.X;
            y = p.Y - p0.Y;
            z = p.Z - p0.Z;
        }


        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double Z
        {
            get { return z; }
            set { z = value; }
        }


        public Vector3D Add(Vector3D v2)
        {
            Vector3D retV = new Vector3D();
            retV.X = this.X + v2.X;
            retV.Y = this.Y + v2.Y;
            retV.Z = this.Z + v2.Z;
            return retV;
        }



        public Vector3D Add(IEnumerable<double> v2)
        {
            Vector3D retV = new Vector3D();
            retV.X = this.X + v2.ElementAt(0);
            retV.Y = this.Y + v2.ElementAt(1);
            retV.Z = this.Z + v2.ElementAt(2);
            return retV;
        }



        public Vector3D Subtract(Vector3D v2)
        {
            Vector3D retV = new Vector3D();
            retV.X = this.X - v2.X;
            retV.Y = this.Y - v2.Y;
            retV.Z = this.Z - v2.Z;
            return retV;
        }



        public Vector3D Subtract(IEnumerable<double> v2)
        {
            Vector3D retV = new Vector3D();
            retV.X = this.X - v2.ElementAt(0);
            retV.Y = this.Y - v2.ElementAt(1);
            retV.Z = this.Z - v2.ElementAt(2);
            return retV;
        }



        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return v1.Add(v2);
        }



        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return v1.Subtract(v2);
        }



        public static Vector3D operator -(Vector3D v1)
        {
            return (new Vector3D()).Subtract(v1);
        }



        public double ScalarProduct(Vector3D v2)
        {
            return (X * v2.X + Y * v2.Y + Z * v2.Z);
        }


        public Vector3D VectorProduct(Vector3D v2)
        {
            Vector3D retV = new Vector3D();
            retV.X = Y * v2.Z - Z * v2.Y;
            retV.Y = -(X * v2.Z - Z * v2.X);
            retV.Z = X * v2.Y - Y * v2.X;
            return retV;
        }



        public static Vector3D operator ^(Vector3D v1, Vector3D v2)
        {
            return v1.VectorProduct(v2);
        }



        public Vector3D Product(double d)
        {
            return new Vector3D(X * d, Y * d, Z * d);
        }


        public static double operator *(Vector3D v1, Vector3D v2)
        {
            return v1.ScalarProduct(v2);
        }


        public static Vector3D operator *(Vector3D v1, double d)
        {
            return v1.Product(d);
        }



        public static Vector3D operator *(double d, Vector3D v1)
        {
            return v1.Product(d);
        }



        public double VectorLength
        {
            get { return Math.Sqrt(x * x + y * y + z * z); }
            set
            {
                if (value == VectorLength)
                {
                    return;
                }
                else if (VectorLength == 0.0d)
                {
                    Exception ex = new Exception("zero-length vector length cant be modified");
                    throw ex;
                }
                else
                {
                    double k = value / VectorLength;
                    x *= k;
                    y *= k;
                    z *= k;
                }
            }
        }



        public IEnumerator<double> GetEnumerator()
        {
            return (IEnumerator<double>)(new double[3] { x, y, z }.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
