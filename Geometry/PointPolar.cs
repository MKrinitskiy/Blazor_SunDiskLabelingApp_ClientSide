using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Drawing;
using Geometry;

namespace Geometry
{
    /// <summary>
    /// Представляет упорядоченную пару координат Х и Y с плавающей запятой, определяющую точку на двумерной плоскости.
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [ComVisible(true)]
    [Serializable]
    public struct   PointPolar
    {
        /// <summary>
        /// Представляет новый экземпляр класса <see cref="T:System.Drawing.PointPolar"/> с неинициализированными данными членов.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public static readonly PointPolar Empty = new PointPolar();
        private double r;
        private double phi;

        /// <summary>
        /// Получает значение, определяющее, пуст ли класс <see cref="T:System.Drawing.PointPolar"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Значениеtrue, если оба свойства <see cref="P:System.Drawing.PointPolar.X"/> и <see cref="P:System.Drawing.PointPolar.Y"/> равны 0, в противном случае возвращается значение false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                if (this.r == 0.0)
                    return this.phi == 0.0;
                else
                    return false;
            }
        }



        public double PhiDegrees
        {
            get
            {
                return this.phi * 180.0d / Math.PI;
            }
            set
            {
                this.Phi = value * Math.PI / 180.0d;
            }
        }



        /// <summary>
        /// Получает или задает координату Х точки <see cref="T:System.Drawing.PointPolar"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Координата Х точки <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public double R
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.r;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                if (value < 0.0d) this.r = 0.0d;
                else this.r = value;
            }
        }

        /// <summary>
        /// Получает или задает координату Y точки <see cref="T:System.Drawing.PointPolar"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Координата Y точки <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public double Phi
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.phi;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this.phi = value;
            }
        }

        static PointPolar()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Drawing.PointPolar"/> с указанными координатами.
        /// </summary>
        /// <param name="r">Положение точки по горизонтали.</param><param name="phi">Положение точки по вертикали.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public PointPolar(double r, double phi)
        {
            this.r = r;
            this.phi = phi;
        }



        public PointPolar(PointF p1)
        {
            this.r = (double)p1.X;
            this.phi = (double)p1.Y;
        }



        public PointPolar(PointD p1)
        {
            this.r = (double)p1.X;
            this.phi = (double)p1.Y;
        }


        public PointPolar(PointD p1, bool convertFromDecartToPolar)
        {
            double dx = p1.X;
            double dy = p1.Y;
            this.r = Math.Sqrt(dx * dx + dy * dy);

            double cosPhi = dx / r;
            double ang = Math.Acos(cosPhi);
            if (dy > 0.0d) ang = 2.0d * Math.PI - ang;
            this.phi = ang;
        }





        public PointPolar(Point p1)
        {
            this.r = (double)p1.X;
            this.phi = (double)p1.Y;
        }



        /// <summary>
        /// Смещает точку <see cref="T:System.Drawing.PointPolar"/> на заданное значение <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Возвращает перенесенную точку <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointPolar"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.Size"/>, определяющий пару чисел, которые нужно добавить к значениям координат <paramref name="pt"/>.</param><filterpriority>3</filterpriority>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointPolar operator +(PointPolar pt, Size sz)
        {
            return PointPolar.Add(pt, sz);
        }

        /// <summary>
        /// Смещает <see cref="T:System.Drawing.PointPolar"/> на отрицательное значение, заданное параметром <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointPolar"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.Size"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param><filterpriority>3</filterpriority>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointPolar operator -(PointPolar pt, Size sz)
        {
            return PointPolar.Subtract(pt, sz);
        }

        /// <summary>
        /// Смещает точка <see cref="T:System.Drawing.PointPolar"/> на указанный размер <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointPolar"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для добавления к координатам X и Y точки <see cref="T:System.Drawing.PointPolar"/>.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointPolar operator +(PointPolar pt, SizeD sz)
        {
            return PointPolar.Add(pt, sz);
        }

        /// <summary>
        /// Смещает точку <see cref="T:System.Drawing.PointPolar"/> на отрицательную величину заданного размера <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointPolar"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static PointPolar operator -(PointPolar pt, SizeD sz)
        {
            return PointPolar.Subtract(pt, sz);
        }

        /// <summary>
        /// Сравнивает две структуры <see cref="T:System.Drawing.PointPolar"/>. Результат определяет, равны или нет значения свойств <see cref="P:System.Drawing.PointPolar.X"/> и <see cref="P:System.Drawing.PointPolar.Y"/> двух структур <see cref="T:System.Drawing.PointPolar"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Значение true, если значения <see cref="P:System.Drawing.PointPolar.X"/> и <see cref="P:System.Drawing.PointPolar.Y"/> левой и правой структур <see cref="T:System.Drawing.PointPolar"/> равны; в противном случае — false.
        /// </returns>
        /// <param name="left">Объект <see cref="T:System.Drawing.PointPolar"/> для сравнения.</param><param name="right">Объект <see cref="T:System.Drawing.PointPolar"/> для сравнения.</param><filterpriority>3</filterpriority>
        public static bool operator ==(PointPolar left, PointPolar right)
        {
            if (left.R == (double)right.R)
                return left.Phi == (double)right.Phi;
            else
                return false;
        }

        /// <summary>
        /// Определяет, равны или нет координаты указанных точек.
        /// </summary>
        /// 
        /// <returns>
        /// Значение true, чтобы указать, что значения <see cref="P:System.Drawing.PointPolar.X"/> и <see cref="P:System.Drawing.PointPolar.Y"/> параметров <paramref name="left"/> и <paramref name="right"/> не равны; в противном случае — false.
        /// </returns>
        /// <param name="left">Объект <see cref="T:System.Drawing.PointPolar"/> для сравнения.</param><param name="right">Объект <see cref="T:System.Drawing.PointPolar"/> для сравнения.</param><filterpriority>3</filterpriority>
        public static bool operator !=(PointPolar left, PointPolar right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Смещает указанную точку <see cref="T:System.Drawing.PointPolar"/> на заданное значение <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointPolar"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.Size"/>, определяющий числа, которые нужно добавить к значениям координат <paramref name="pt"/>.</param>
        public static PointPolar Add(PointPolar pt, Size sz)
        {
            return new PointPolar(pt.R + (double)sz.Width, pt.Phi + (double)sz.Height);
        }

        /// <summary>
        /// Смещает <see cref="T:System.Drawing.PointPolar"/> на отрицательную величину заданного размера.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointPolar"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.Size"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        public static PointPolar Subtract(PointPolar pt, Size sz)
        {
            return new PointPolar(pt.R - (double)sz.Width, pt.Phi - (double)sz.Height);
        }

        /// <summary>
        /// Смещает указанную точку <see cref="T:System.Drawing.PointPolar"/> на заданное значение <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointPolar"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.SizeD"/>, определяющий числа, которые нужно добавить к значениям координат <paramref name="pt"/>.</param>
        public static PointPolar Add(PointPolar pt, SizeD sz)
        {
            return new PointPolar(pt.R + sz.Width, pt.Phi + sz.Height);
        }

        /// <summary>
        /// Смещает <see cref="T:System.Drawing.PointPolar"/> на отрицательную величину заданного размера.
        /// </summary>
        /// 
        /// <returns>
        /// Смещенная точка <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <param name="pt">Класс <see cref="T:System.Drawing.PointPolar"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
        public static PointPolar Subtract(PointPolar pt, SizeD sz)
        {
            return new PointPolar(pt.R - sz.Width, pt.Phi - sz.Height);
        }

        /// <summary>
        /// Определяет, содержит или нет объект <see cref="T:System.Drawing.PointPolar"/> те же координаты, что и указанный объект <see cref="T:System.Object"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Метод возвращает значение true, если <paramref name="obj"/> является <see cref="T:System.Drawing.PointPolar"/> и имеет такие же координаты, как и <see cref="T:System.Drawing.Point"/>.
        /// </returns>
        /// <param name="obj">Политика <see cref="T:System.Object"/> для проверки.</param><filterpriority>1</filterpriority>
        public override bool Equals(object obj)
        {
            if (!(obj is PointPolar))
                return false;
            PointPolar PointPolar = (PointPolar)obj;
            if (PointPolar.R == (double)this.R && PointPolar.Phi == (double)this.Phi)
                return PointPolar.GetType().Equals(this.GetType());
            else
                return false;
        }

        /// <summary>
        /// Возвращает хэш-код для этой структуры <see cref="T:System.Drawing.PointPolar"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Целое значение, указывающее значение хэша для этой структуры <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Преобразует объект <see cref="T:System.Drawing.PointPolar"/> в строку, доступную для чтения.
        /// </summary>
        /// 
        /// <returns>
        /// Строка, представляющая структуру <see cref="T:System.Drawing.PointPolar"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{R={0}, phi={1}}}", new object[2]
      {
        this.r,
        this.phi
      });
        }




        public static double Distance(PointPolar p1, PointPolar p2)
        {
            PointD decartPointD1 = new PointD();
            decartPointD1.X = p1.R * Math.Cos(p1.Phi);
            decartPointD1.Y = p1.R * Math.Sin(p1.Phi);
            PointD decartPointD2 = new PointD();
            decartPointD2.X = p2.R * Math.Cos(p2.Phi);
            decartPointD2.Y = p2.R * Math.Sin(p2.Phi);

            double dx = decartPointD2.X - decartPointD1.X;
            double dy = decartPointD2.Y - decartPointD1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }




        public PointD PointD()
        {
            PointD decartPointD = new PointD();
            decartPointD.X = R * Math.Cos(Phi);
            decartPointD.Y = R * Math.Sin(Phi);
            return decartPointD;
        }




        public double Distance(PointPolar p2)
        {
            return Distance(this, p2);
        }



        public static PointPolar Sum(IEnumerable<PointPolar> source)
        {
            PointPolar sum = new PointPolar(0.0d, 0.0d);
            if (source == null) return sum;
            checked
            {
                foreach (PointPolar v in source)
                    sum += new SizeD(v.R, v.Phi);
            }
            return sum;
        }


        public static PointPolar Mult(PointPolar pt, double d)
        {
            return new PointPolar(pt.R * d, pt.Phi * d);
        }


        public static PointPolar Div(PointPolar pt, double d)
        {
            if (d == 0.0d) return pt;
            return new PointPolar(pt.R / d, pt.Phi / d);
        }



        public static PointPolar operator /(PointPolar pt, double d)
        {
            return PointPolar.Div(pt, d);
        }


        public static PointPolar operator *(PointPolar pt, double d)
        {
            return PointPolar.Mult(pt, d);
        }



        public void CropAngle(bool symmetricScale = false)
        {
            double retAngle = this.Phi;
            double angleInput = this.Phi;

            if (symmetricScale)
            {
                double retAngleSignif = Math.Asin(Math.Sin(angleInput));
                if (Math.Cos(angleInput) < 0.0d) retAngle = (double)Math.Sign(retAngleSignif) * Math.PI - retAngleSignif;
                else retAngle = retAngleSignif;
            }
            else
            {
                double retAnglesignif = Math.Acos(Math.Cos(angleInput));
                if (Math.Sin(angleInput) < 0.0d) retAngle = 2.0d * Math.PI - retAnglesignif;
                else retAngle = retAnglesignif;
            }

            this.Phi = retAngle;
        }




        public static double CropAngleDegrees(double angle)
        {
            double retAngle = angle;
            double angleInput = (angle / 180.0d) * Math.PI;

            
            double retAnglesignif = Math.Acos(Math.Cos(angleInput));
            if (Math.Sin(angleInput) < 0.0d) retAngle = 2.0d * Math.PI - retAnglesignif;
            else retAngle = retAnglesignif;

            retAngle = (retAngle/Math.PI)*180.0d;

            return retAngle;
        }





        public static double CropAngleRad(double angle)
        {
            double retAngle = angle;
            double angleInput = angle;


            double retAnglesignif = Math.Acos(Math.Cos(angleInput));
            if (Math.Sin(angleInput) < 0.0d) retAngle = 2.0d * Math.PI - retAnglesignif;
            else retAngle = retAnglesignif;
            
            return retAngle;
        }



        public static double CropAngleRad(double angleRad, bool symmetricScale = false)
        {
            double retAngle = angleRad;
            double angleInput = angleRad;

            if (symmetricScale)
            {
                double retAngleSignif = Math.Asin(Math.Sin(angleInput));
                if (Math.Cos(angleInput) < 0.0d) retAngle = (double)Math.Sign(retAngleSignif) * Math.PI - retAngleSignif;
                else retAngle = retAngleSignif;
            }
            else
            {
                double retAnglesignif = Math.Acos(Math.Cos(angleInput));
                if (Math.Sin(angleInput) < 0.0d) retAngle = 2.0d * Math.PI - retAnglesignif;
                else retAngle = retAnglesignif;
            }

            return retAngle;
        }

    }
}
