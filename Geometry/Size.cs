using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;


namespace Geometry
{
    /// <summary>
    /// Содержит упорядоченную пару чисел с плавающей запятой, обычно ширину и высоту прямоугольника.
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [ComVisible(true)]
    //[TypeConverter(typeof (SizeFConverter))]
    [Serializable]
    public class Size
    {
        /// <summary>
        /// Получает объект класса SizeD <see cref="T:Geometry.SizeD"/>, имеющую значения <see cref="P:System.Drawing.SizeD.Height"/> и <see cref="P:System.Drawing.SizeD.Width"/>, равные 0.
        /// </summary>
        /// 
        /// <returns>
        /// Структура <see cref="T:System.Drawing.SizeD"/>, имеющая значения <see cref="P:System.Drawing.SizeD.Height"/> и <see cref="P:System.Drawing.SizeD.Width"/>, равные 0.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public static readonly Size Empty = new Size();
        private int width = 0;
        private int height = 0;

        public double DiagonalLength => Math.Sqrt(width * width + height * height);

        public Size() { }

        /// <summary>
        /// Получает значение, указывающее, имеет ли эта структура <see cref="T:Geometry.Size"/> нулевую ширину и высоту.
        /// </summary>
        /// 
        /// <returns>
        /// Это свойство возвращает значение true, когда эта структура <see cref="T:Geometry.Size"/> имеет нулевую ширину и высоту; в противном случае возвращается значение false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public bool IsEmpty
        {
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            get
            {
                if ((double)this.width == 0)
                    return (double)this.height == 0;
                else
                    return false;
            }
        }

        /// <summary>
        /// Получает или задает горизонтальный компонент элемента <see cref="T:Geometry.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Горизонтальный компонент элемента <see cref="T:System.Drawing.SizeD"/>, обычно измеряемый в пикселях.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public int Width
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.width;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this.width = value;
            }
        }

        /// <summary>
        /// Получает или задает вертикальный компонент элемента <see cref="T:Geometry.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Вертикальный компонент элемента <see cref="T:Geometry.Size"/>, обычно измеряемый в пикселях.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public int Height
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.height;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this.height = value;
            }
        }


        public Size Clone()
        {
            return new Size()
            {
                width = this.width,
                height = this.height
            };
        }

        
        public SizeD ToSizeD()
        {
            return new SizeD()
            {
                Width = this.width,
                Height = this.height
            };
        }


        /// <summary>
        /// Инициализирует новый экземпляр структуры <see cref="T:System.Drawing.SizeD"/> из указанной структуры <see cref="T:System.Drawing.PointF"/>.
        /// </summary>
        /// <param name="pt">Структура <see cref="T:System.Drawing.PointF"/>, из которой инициализируется эта структура <see cref="T:System.Drawing.SizeD"/>.</param>
        public Size(Point pt)
        {
            this.width = pt.X;
            this.height = pt.Y;
        }


        public Size(Point pt1, Point pt2)
        {
            this.width = Math.Abs(pt1.X - pt2.X);
            this.height = Math.Abs(pt1.Y - pt2.Y);
        }



        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Преобразует заданную структуру <see cref="T:System.Drawing.SizeD"/> в структуру <see cref="T:System.Drawing.PointF"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Структура <see cref="T:System.Drawing.PointF"/>, которая является результатом преобразования, выполненного с помощью этого оператора.
        /// </returns>
        /// <param name="size">Преобразуемая структура <see cref="T:System.Drawing.SizeD"/>.</param><filterpriority>3</filterpriority>
        public static explicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        /// <summary>
        /// Прибавляет ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> к ширине и высоте другой структуры <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Структура <see cref="T:System.Drawing.Size"/>, получаемая в результате операции сложения.
        /// </returns>
        /// <param name="sz1">Первая складываемая структура <see cref="T:System.Drawing.SizeD"/>.</param><param name="sz2">Вторая складываемая структура <see cref="T:System.Drawing.SizeD"/>.</param><filterpriority>3</filterpriority>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static Size operator +(Size sz1, Size sz2)
        {
            return Size.Add(sz1, sz2);
        }

        /// <summary>
        /// Вычитает ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> из ширины и высоты другой структуры <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Структура <see cref="T:System.Drawing.SizeD"/>, полученная в результате операции вычитания.
        /// </returns>
        /// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора вычитания.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора вычитания.</param><filterpriority>3</filterpriority>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static Size operator -(Size sz1, Size sz2)
        {
            return Size.Subtract(sz1, sz2);
        }



        public static Size operator *(Size sz1, int dVal)
        {
            return Size.Mul(sz1, dVal);
        }



        public static SizeD operator *(Size sz1, double dVal)
        {
            return SizeD.Mul(sz1.ToSizeD(), dVal);
        }



        public static SizeD operator /(Size sz1, int dVal)
        {
            return SizeD.Div(sz1.ToSizeD(), (double)dVal);
        }



        /// <summary>
        /// Проверяет, действительно ли две структуры <see cref="T:System.Drawing.SizeD"/> эквивалентны.
        /// </summary>
        /// 
        /// <returns>
        /// Этот оператор возвращает значение true, если параметры <paramref name="sz1"/> и <paramref name="sz2"/> имеют равные ширину и высоту; в противном случае возвращается значение false.
        /// </returns>
        /// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора равенства.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора равенства.</param><filterpriority>3</filterpriority>
        public static bool operator ==(Size sz1, Size sz2)
        {
            if ((double)sz1.Width == (double)sz2.Width)
                return (double)sz1.Height == (double)sz2.Height;
            else
                return false;
        }

        /// <summary>
        /// Проверяет, различны ли две структуры <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Этот оператор возвращает значение true, если параметры <paramref name="sz1"/> и <paramref name="sz2"/> отличаются по ширине или по высоте, и значение false, если параметры <paramref name="sz1"/> и <paramref name="sz2"/> равны.
        /// </returns>
        /// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора неравенства.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора неравенства.</param><filterpriority>3</filterpriority>
        public static bool operator !=(Size sz1, Size sz2)
        {
            return !(sz1 == sz2);
        }

        /// <summary>
        /// Прибавляет ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> к ширине и высоте другой структуры <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Структура <see cref="T:System.Drawing.SizeD"/>, получаемая в результате операции сложения.
        /// </returns>
        /// <param name="sz1">Первая складываемая структура <see cref="T:System.Drawing.SizeD"/>.</param><param name="sz2">Вторая складываемая структура <see cref="T:System.Drawing.SizeD"/>.</param>
        public static Size Add(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        /// <summary>
        /// Вычитает ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> из ширины и высоты другой структуры <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Структура <see cref="T:System.Drawing.SizeD"/>, полученная в результате операции вычитания.
        /// </returns>
        /// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора вычитания.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора вычитания.</param>
        public static Size Subtract(Size sz1, Size sz2)
        {
            return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }



        public static Size Mul(Size sz1, int iVal)
        {
            return new Size(sz1.Width * iVal, sz1.Height * iVal);
        }


        
        public override bool Equals(object obj)
        {
            if (!(obj is Size))
                return false;
            Size size = (Size)obj;
            if (size.Width == this.Width && size.Height == this.Height)
                return size.GetType().Equals(this.GetType());
            else
                return false;
        }



        /// <summary>
        /// Возвращает хэш-код для этой структуры <see cref="T:System.Drawing.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Целое значение, указывающее значение хэша для этой структуры <see cref="T:System.Drawing.Size"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        

        public SizeF ToSizeF()
        {
            return new SizeF((float)this.width, (float)this.height);
        }



        /// <summary>
        /// Создает удобную для восприятия строку, представляющую эту структуру <see cref="T:System.Drawing.SizeD"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Строка, представляющая эту структуру <see cref="T:System.Drawing.SizeD"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
        public override string ToString()
        {
            return "{Width=" + this.width.ToString((IFormatProvider)CultureInfo.CurrentCulture) + ", Height=" + this.height.ToString((IFormatProvider)CultureInfo.CurrentCulture) + "}";
        }
    }
}
