using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Geometry
{
    /// <summary>
    /// Представляет упорядоченную пару координат Х и Y с плавающей запятой, определяющую точку на двумерной плоскости.
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [ComVisible(true)]
    [Serializable]
    public struct PointD
    {
        /// <summary>
        /// Представляет новый экземпляр класса <see cref="T:System.Drawing.PointD"/> с неинициализированными данными членов.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public static readonly PointD Empty = new PointD();
        private double x;
        private double y;
        private bool nullPoint;

        /// <summary>
        /// Получает значение, определяющее, пуст ли класс <see cref="T:System.Drawing.PointD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Значение true, если оба свойства <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> равны 0, в противном случае возвращается значение false.
        /// </returns>
        /// <filterpriority>1</filterpriority>

        //[Browsable(false)]
        //public bool IsEmpty
        //{
        //    get
        //    {
        //        if (this.x == 0.0)
        //            return this.y == 0.0;
        //        else
        //            return false;
        //    }
        //}


        public bool GetIsNull()
        { return nullPoint; }

        /// <summary>
        /// Получает значение, определяющее, пуст ли класс <see cref="T:System.Drawing.PointD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Значение true, если оба свойства <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> равны 0, в противном случае возвращается значение false.
        /// </returns>
        /// <filterpriority>1</filterpriority>

        
        public void SetIsNull(bool value)
        {
            this.nullPoint = value;
            this.X = 0.0d;
            this.Y = 0.0d;
        }



        /// <summary>
        /// Получает или задает координату Х точки <see cref="T:System.Drawing.PointD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Координата Х точки <see cref="T:System.Drawing.PointD"/>.
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
        /// Получает или задает координату Y точки <see cref="T:System.Drawing.PointD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Координата Y точки <see cref="T:System.Drawing.PointD"/>.
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

        static PointD()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Drawing.PointD"/> с указанными координатами.
        /// </summary>
        /// <param name="x">Положение точки по горизонтали.</param><param name="y">Положение точки по вертикали.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public PointD(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.nullPoint = false;
        }



        public PointD(PointF p1)
        {
            this.x = (double)p1.X;
            this.y = (double)p1.Y;
            this.nullPoint = false;
        }



        public PointD(PointD p1)
        {
            this.x = (double)p1.X;
            this.y = (double)p1.Y;
            this.nullPoint = false;
        }




        public PointD(Point p1)
        {
            this.x = (double)p1.X;
            this.y = (double)p1.Y;
            this.nullPoint = false;
        }





        public static PointD nullPointD()
        {
            PointD pt = new PointD();
            pt.SetIsNull(true);
            return pt;
        }



        public static bool isNull(PointD pt)
        {
            return pt.GetIsNull();
        }


        /// <summary>
        /// Смещает точку <see cref="T:System.Drawing.PointD"/> на заданное значение <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Возвращает перенесенную точку <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.Size"/>, определяющий пару чисел, которые нужно добавить к значениям координат <paramref name="pt"/>.</param><filterpriority>3</filterpriority>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointD operator +(PointD pt, Size sz)
        {
            return PointD.Add(pt, sz);
        }



        public static PointD operator +(PointD pt, IEnumerable<double> DenseVectorShift)
        {
            PointD retPt = new PointD(pt);
            retPt.X += DenseVectorShift.ElementAt(0);
            retPt.Y += DenseVectorShift.ElementAt(1);
            return retPt;
        }




        public static PointD operator +(IEnumerable<double> DenseVectorShift, PointD pt)
        {
            PointD retPt = new PointD(pt);
            retPt.X += DenseVectorShift.ElementAt(0);
            retPt.Y += DenseVectorShift.ElementAt(1);
            return retPt;
        }



        /// <summary>
        /// Смещает <see cref="T:System.Drawing.PointD"/> на отрицательное значение, заданное параметром <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.Size"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param><filterpriority>3</filterpriority>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointD operator -(PointD pt, Size sz)
        {
            return PointD.Subtract(pt, sz);
        }




        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointD operator -(PointD pt, PointD pt0)
        {
            return pt + new double[]{-pt0.X, -pt0.Y};
        }




        /// <summary>
        /// Смещает точка <see cref="T:System.Drawing.PointD"/> на указанный размер <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для добавления к координатам X и Y точки <see cref="T:System.Drawing.PointD"/>.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointD operator +(PointD pt, SizeD sz)
        {
            return PointD.Add(pt, sz);
        }

        /// <summary>
        /// Смещает точку <see cref="T:System.Drawing.PointD"/> на отрицательную величину заданного размера <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointD operator -(PointD pt, SizeD sz)
        {
            return PointD.Subtract(pt, sz);
        }

        /// <summary>
        /// Сравнивает две структуры <see cref="T:System.Drawing.PointD"/>. Результат определяет, равны или нет значения свойств <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> двух структур <see cref="T:System.Drawing.PointD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Значение true, если значения <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> левой и правой структур <see cref="T:System.Drawing.PointD"/> равны; в противном случае — false.
        /// </returns>
        /// <param name="left">Объект <see cref="T:System.Drawing.PointD"/> для сравнения.</param><param name="right">Объект <see cref="T:System.Drawing.PointD"/> для сравнения.</param><filterpriority>3</filterpriority>
        public static bool operator ==(PointD left, PointD right)
        {
            if (left.X == (double)right.X)
                return left.Y == (double)right.Y;
            else
                return false;
        }

        /// <summary>
        /// Определяет, равны или нет координаты указанных точек.
        /// </summary>
        /// 
        /// <returns>
        /// Значение true, чтобы указать, что значения <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> параметров <paramref name="left"/> и <paramref name="right"/> не равны; в противном случае — false.
        /// </returns>
        /// <param name="left">Объект <see cref="T:System.Drawing.PointD"/> для сравнения.</param><param name="right">Объект <see cref="T:System.Drawing.PointD"/> для сравнения.</param><filterpriority>3</filterpriority>
        public static bool operator !=(PointD left, PointD right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Смещает указанную точку <see cref="T:System.Drawing.PointD"/> на заданное значение <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.Size"/>, определяющий числа, которые нужно добавить к значениям координат <paramref name="pt"/>.</param>
        public static PointD Add(PointD pt, Size sz)
        {
            return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
        }

        /// <summary>
        /// Смещает <see cref="T:System.Drawing.PointD"/> на отрицательную величину заданного размера.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.Size"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        public static PointD Subtract(PointD pt, Size sz)
        {
            return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
        }

        /// <summary>
        /// Смещает указанную точку <see cref="T:System.Drawing.PointD"/> на заданное значение <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.SizeD"/>, определяющий числа, которые нужно добавить к значениям координат <paramref name="pt"/>.</param>
        public static PointD Add(PointD pt, SizeD sz)
        {
            return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
        }

        /// <summary>
        /// Смещает <see cref="T:System.Drawing.PointD"/> на отрицательную величину заданного размера.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        public static PointD Subtract(PointD pt, SizeD sz)
        {
            return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
        }

        /// <summary>
        /// Определяет, содержит или нет объект <see cref="T:System.Drawing.PointD"/> те же координаты, что и указанный объект <see cref="T:System.Object"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Метод возвращает значение true, если <paramref name="obj"/> является <see cref="T:System.Drawing.PointD"/> и имеет такие же координаты, как и <see cref="T:System.Drawing.Point"/>.
        /// </returns>
        /// <param name="obj">Политика <see cref="T:System.Object"/> для проверки.</param><filterpriority>1</filterpriority>
        public override bool Equals(object obj)
        {
            if (!(obj is PointD))
                return false;
            PointD PointD = (PointD)obj;
            if (PointD.X == (double)this.X && PointD.Y == (double)this.Y)
                return PointD.GetType().Equals(this.GetType());
            else
                return false;
        }

        /// <summary>
        /// Возвращает хэш-код для этой структуры <see cref="T:System.Drawing.PointD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Целое значение, указывающее значение хэша для этой структуры <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Преобразует объект <see cref="T:System.Drawing.PointD"/> в строку, доступную для чтения.
        /// </summary>
        /// 
        /// <returns>
        /// Строка, представляющая структуру <see cref="T:System.Drawing.PointD"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[2]
      {
        this.x,
        this.y
      });
        }




        public static double DistanceTo(PointD p1, PointD p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx*dx + dy*dy);
        }



        public double DistanceTo(PointD p2)
        {
            return DistanceTo(this, p2);
        }



        public double DistanceToLine(LineDescription2D l1)
        {
            //PointD vect1 = new PointD(l1.directionVector[0], l1.directionVector[1]);
            //PointD vect2 = l1.p0 - new SizeD(this);
            //double retDist = (vect1.X * vect2.Y - vect1.Y * vect2.X) / (l1.directionVectorLength);
            //return Math.Abs(retDist);
            return l1.Distance(this);
        }



        public PointF PointF()
        {
            return new PointF((float) this.X, (float) this.Y);
        }


        public Point Point()
        {
            return new Point((int)this.X, (int)this.Y);
        }


        public static PointD Sum(IEnumerable<PointD> source)
        {
            PointD sum = new PointD(0.0d, 0.0d);
            if (source == null) return sum;
            checked
            {
                foreach (PointD v in source)
                    sum += new SizeD(v);
            }
            return sum;
        }


        public static PointD Mult(PointD pt, double d)
        {
            return new PointD(pt.X * d, pt.Y * d);
        }


        public static PointD Div(PointD pt, double d)
        {
            if (d == 0.0d) return pt;
            return new PointD(pt.X / d, pt.Y / d);
        }



        public static PointD operator /(PointD pt, double d)
        {
            return PointD.Div(pt, d);
        }


        public static PointD operator *(PointD pt, double d)
        {
            return PointD.Mult(pt, d);
        }

    }
}
