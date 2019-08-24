using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    [ComVisible(true)]
    [Serializable]
    public class Point3D
    {
        /// <summary>
        /// Представляет новый экземпляр класса <see cref="T:System.Drawing.Point3D"/> с неинициализированными данными членов.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public static readonly Point3D Empty = new Point3D();
        private double x;
        private double y;
        private double z;
        private bool nullPoint;

        /// <summary>
        /// Получает значение, определяющее, пуст ли класс <see cref="T:System.Drawing.Point3D"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Значениеtrue, если оба свойства <see cref="P:System.Drawing.Point3D.X"/> и <see cref="P:System.Drawing.Point3D.Y"/> равны 0, в противном случае возвращается значение false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return (this.x == 0.0) && (this.y == 0.0) && (this.z == 0.0);
            }
        }


        public bool IsNull
        {
            get { return nullPoint; }
            set
            {
                this.nullPoint = value;
                this.X = 0.0d;
                this.Y = 0.0d;
                this.Z = 0.0d;
            }
        }



        /// <summary>
        /// Получает или задает координату Х точки <see cref="T:System.Drawing.Point3D"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Координата Х точки <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public double X
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.x;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Получает или задает координату Y точки <see cref="T:System.Drawing.Point3D"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Координата Y точки <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public double Y
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.y;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this.y = value;
            }
        }



        public double Z
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.z;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this.z = value;
            }
        }



        public Point3D()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Drawing.Point3D"/> с указанными координатами.
        /// </summary>
        /// <param name="x">Положение точки по горизонтали.</param><param name="y">Положение точки по вертикали.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.nullPoint = false;
        }



        public Point3D(Point3D p1)
        {
            this.x = (double)p1.X;
            this.y = (double)p1.Y;
            this.z = (double)p1.Z;
            this.nullPoint = false;
        }




        public static Point3D nullPoint3D()
        {
            Point3D pt = new Point3D();
            pt.IsNull = true;
            return pt;
        }



        public static bool isNull(Point3D pt)
        {
            return pt.IsNull;
        }


        /// <summary>
        /// Смещает точку <see cref="T:System.Drawing.Point3D"/> на заданное значение <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Возвращает перенесенную точку <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.Point3D"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.Size"/>, определяющий пару чисел, которые нужно добавить к значениям координат <paramref name="pt"/>.</param><filterpriority>3</filterpriority>
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        //public static Point3D operator +(Point3D pt, Size sz)
        //{
        //    return Point3D.Add(pt, sz);
        //}



        public static Point3D operator +(Point3D pt, IEnumerable<double> DenseVectorShift)
        {
            Point3D retPt = new Point3D(pt);
            retPt.X += DenseVectorShift.ElementAt(0);
            retPt.Y += DenseVectorShift.ElementAt(1);
            retPt.Z += DenseVectorShift.ElementAt(2);
            return retPt;
        }




        public static Point3D operator +(IEnumerable<double> DenseVectorShift, Point3D pt)
        {
            Point3D retPt = new Point3D(pt);
            retPt.X += DenseVectorShift.ElementAt(0);
            retPt.Y += DenseVectorShift.ElementAt(1);
            retPt.Z += DenseVectorShift.ElementAt(2);
            return retPt;
        }



        public static Point3D operator -(Point3D pt, IEnumerable<double> DenseVectorShift)
        {
            Point3D retPt = new Point3D(pt);
            retPt.X -= DenseVectorShift.ElementAt(0);
            retPt.Y -= DenseVectorShift.ElementAt(1);
            retPt.Z -= DenseVectorShift.ElementAt(2);
            return retPt;
        }





        /// <summary>
        /// Смещает <see cref="T:System.Drawing.Point3D"/> на отрицательное значение, заданное параметром <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.Point3D"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.Size"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param><filterpriority>3</filterpriority>
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        //public static Point3D operator -(Point3D pt, Size sz)
        //{
        //    return Point3D.Subtract(pt, sz);
        //}



        /// <summary>
        /// Смещает точка <see cref="T:System.Drawing.Point3D"/> на указанный размер <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.Point3D"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для добавления к координатам X и Y точки <see cref="T:System.Drawing.Point3D"/>.</param>
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        //public static Point3D operator +(Point3D pt, SizeD sz)
        //{
        //    return Point3D.Add(pt, sz);
        //}

        /// <summary>
        /// Смещает точку <see cref="T:System.Drawing.Point3D"/> на отрицательную величину заданного размера <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.Point3D"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        //public static Point3D operator -(Point3D pt, SizeD sz)
        //{
        //    return Point3D.Subtract(pt, sz);
        //}

        /// <summary>
        /// Сравнивает две структуры <see cref="T:System.Drawing.Point3D"/>. Результат определяет, равны или нет значения свойств <see cref="P:System.Drawing.Point3D.X"/> и <see cref="P:System.Drawing.Point3D.Y"/> двух структур <see cref="T:System.Drawing.Point3D"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Значение true, если значения <see cref="P:System.Drawing.Point3D.X"/> и <see cref="P:System.Drawing.Point3D.Y"/> левой и правой структур <see cref="T:System.Drawing.Point3D"/> равны; в противном случае — false.
        /// </returns>
        /// <param name="left">Объект <see cref="T:System.Drawing.Point3D"/> для сравнения.</param><param name="right">Объект <see cref="T:System.Drawing.Point3D"/> для сравнения.</param><filterpriority>3</filterpriority>
        public static bool operator ==(Point3D left, Point3D right)
        {
            return ((left.X == right.X) && (left.Y == right.Y) && (left.Z == right.Z));
            //if (left.X == (double)right.X)
            //    return left.Y == (double)right.Y;
            //else
            //    return false;
        }

        /// <summary>
        /// Определяет, равны или нет координаты указанных точек.
        /// </summary>
        /// 
        /// <returns>
        /// Значение true, чтобы указать, что значения <see cref="P:System.Drawing.Point3D.X"/> и <see cref="P:System.Drawing.Point3D.Y"/> параметров <paramref name="left"/> и <paramref name="right"/> не равны; в противном случае — false.
        /// </returns>
        /// <param name="left">Объект <see cref="T:System.Drawing.Point3D"/> для сравнения.</param><param name="right">Объект <see cref="T:System.Drawing.Point3D"/> для сравнения.</param><filterpriority>3</filterpriority>
        public static bool operator !=(Point3D left, Point3D right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Смещает указанную точку <see cref="T:System.Drawing.Point3D"/> на заданное значение <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.Point3D"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.Size"/>, определяющий числа, которые нужно добавить к значениям координат <paramref name="pt"/>.</param>
        //public static Point3D Add(Point3D pt, Size sz)
        //{
        //    return new Point3D(pt.X + sz.Width, pt.Y + sz.Height);
        //}

        /// <summary>
        /// Смещает <see cref="T:System.Drawing.Point3D"/> на отрицательную величину заданного размера.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.Point3D"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.Size"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        //public static Point3D Subtract(Point3D pt, Size sz)
        //{
        //    return new Point3D(pt.X - sz.Width, pt.Y - sz.Height);
        //}

        /// <summary>
        /// Смещает указанную точку <see cref="T:System.Drawing.Point3D"/> на заданное значение <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.Point3D"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.SizeD"/>, определяющий числа, которые нужно добавить к значениям координат <paramref name="pt"/>.</param>
        //public static Point3D Add(Point3D pt, SizeD sz)
        //{
        //    return new Point3D(pt.X + sz.Width, pt.Y + sz.Height);
        //}

        /// <summary>
        /// Смещает <see cref="T:System.Drawing.Point3D"/> на отрицательную величину заданного размера.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.Point3D"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        //public static Point3D Subtract(Point3D pt, SizeD sz)
        //{
        //    return new Point3D(pt.X - sz.Width, pt.Y - sz.Height);
        //}

        /// <summary>
        /// Определяет, содержит или нет объект <see cref="T:System.Drawing.Point3D"/> те же координаты, что и указанный объект <see cref="T:System.Object"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Метод возвращает значение true, если <paramref name="obj"/> является <see cref="T:System.Drawing.Point3D"/> и имеет такие же координаты, как и <see cref="T:System.Drawing.Point"/>.
        /// </returns>
        /// <param name="obj">Политика <see cref="T:System.Object"/> для проверки.</param><filterpriority>1</filterpriority>
        public override bool Equals(object obj)
        {
            if (!(obj is Point3D))
                return false;
            Point3D pt3D = (Point3D)obj;
            if ((pt3D.X == this.X) && (pt3D.Y == this.Y) && (pt3D.Z == this.Z))
                return pt3D.GetType().Equals(this.GetType());
            else
                return false;
        }

        /// <summary>
        /// Возвращает хэш-код для этой структуры <see cref="T:System.Drawing.Point3D"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Целое значение, указывающее значение хэша для этой структуры <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Преобразует объект <see cref="T:System.Drawing.Point3D"/> в строку, доступную для чтения.
        /// </summary>
        /// 
        /// <returns>
        /// Строка, представляющая структуру <see cref="T:System.Drawing.Point3D"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}, Z={2}}}", new object[3]
            {
                this.x,
                this.y,
                this.z
            });
        }




        public static double Distance(Point3D p1, Point3D p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double dz = p2.Z - p1.Z;
            return Math.Sqrt(dx*dx + dy*dy + dz*dz);
        }



        public double Distance(Point3D p2)
        {
            return Distance(this, p2);
        }



        public double Distance(LineDescription3D l1)
        {
            return l1.Distance(this);
        }




        public double Distance(Plane3D plane)
        {
            return plane.Distance(this);
        }



        //public static Point3D Sum(IEnumerable<Point3D> source)
        //{
        //    Point3D sum = new Point3D(0.0d, 0.0d, 0.0d);
        //    if (source == null) return sum;
        //    checked
        //    {
        //        foreach (Point3D v in source)
        //            sum += new SizeD(v);
        //    }
        //    return sum;
        //}




        //public static Point3D Mult(Point3D pt, double d)
        //{
        //    return new Point3D(pt.X * d, pt.Y * d);
        //}


        //public static Point3D Div(Point3D pt, double d)
        //{
        //    if (d == 0.0d) return pt;
        //    return new Point3D(pt.X / d, pt.Y / d);
        //}



        //public static Point3D operator /(Point3D pt, double d)
        //{
        //    return Point3D.Div(pt, d);
        //}


        //public static Point3D operator *(Point3D pt, double d)
        //{
        //    return Point3D.Mult(pt, d);
        //}


    }
}
