using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Geometry
{
  /// <summary>
  /// Содержит упорядоченную пару чисел с плавающей запятой, обычно ширину и высоту прямоугольника.
  /// </summary>
  /// <filterpriority>1</filterpriority>
  [ComVisible(true)]
  //[TypeConverter(typeof (SizeFConverter))]
  [Serializable]
  public struct SizeD
  {
    /// <summary>
    /// Получает структуру <see cref="T:System.Drawing.SizeD"/>, имеющую значения <see cref="P:System.Drawing.SizeD.Height"/> и <see cref="P:System.Drawing.SizeD.Width"/>, равные 0.
    /// </summary>
    /// 
    /// <returns>
    /// Структура <see cref="T:System.Drawing.SizeD"/>, имеющая значения <see cref="P:System.Drawing.SizeD.Height"/> и <see cref="P:System.Drawing.SizeD.Width"/>, равные 0.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static readonly SizeD Empty = new SizeD();
    private double width;
    private double height;

    /// <summary>
    /// Получает значение, указывающее, имеет ли эта структура <see cref="T:System.Drawing.SizeD"/> нулевую ширину и высоту.
    /// </summary>
    /// 
    /// <returns>
    /// Это свойство возвращает значение true, когда эта структура <see cref="T:System.Drawing.SizeD"/> имеет нулевую ширину и высоту; в противном случае возвращается значение false.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    [Browsable(false)]
    public bool IsEmpty
    {
      [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] get
      {
        if ((double) this.width == 0.0)
          return (double) this.height == 0.0;
        else
          return false;
      }
    }

    /// <summary>
    /// Получает или задает горизонтальный компонент этой структуры <see cref="T:System.Drawing.SizeD"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Горизонтальный компонент этой структуры <see cref="T:System.Drawing.SizeD"/>, обычно измеряемый в пикселях.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public double Width
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.width;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.width = value;
      }
    }

    /// <summary>
    /// Получает или задает вертикальный компонент этой структуры <see cref="T:System.Drawing.SizeD"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Вертикальный компонент этой структуры <see cref="T:System.Drawing.SizeD"/>, обычно измеряемый в пикселях.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public double Height
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.height;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.height = value;
      }
    }

    static SizeD()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр структуры <see cref="T:System.Drawing.SizeD"/> из указанной существующей структуры <see cref="T:System.Drawing.SizeD"/>.
    /// </summary>
    /// <param name="size">Структура <see cref="T:System.Drawing.SizeD"/>, на основе которой будет создана новая структура <see cref="T:System.Drawing.SizeD"/>.</param>
    public SizeD(SizeD size)
    {
      this.width = size.width;
      this.height = size.height;
    }

    public SizeD(Size size)
    {
        this.width = (double)size.Width;
        this.height = (double)size.Height;
    }


    public SizeD(SizeF size)
    {
        this.width = (double)size.Width;
        this.height = (double)size.Height;
    }



    /// <summary>
    /// Инициализирует новый экземпляр структуры <see cref="T:System.Drawing.SizeD"/> из указанной структуры <see cref="T:System.Drawing.PointF"/>.
    /// </summary>
    /// <param name="pt">Структура <see cref="T:System.Drawing.PointF"/>, из которой инициализируется эта структура <see cref="T:System.Drawing.SizeD"/>.</param>
    public SizeD(PointD pt)
    {
      this.width = pt.X;
      this.height = pt.Y;
    }


    public SizeD(PointD pt1, PointD pt2)
    {
        this.width = Math.Abs(pt1.X-pt2.X);
        this.height = Math.Abs(pt1.Y-pt2.Y);
    }



    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public SizeD(double width, double height)
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
    public static explicit operator PointD(SizeD size)
    {
      return new PointD(size.Width, size.Height);
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
    public static SizeD operator +(SizeD sz1, SizeD sz2)
    {
      return SizeD.Add(sz1, sz2);
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
    public static SizeD operator -(SizeD sz1, SizeD sz2)
    {
      return SizeD.Subtract(sz1, sz2);
    }



    public static SizeD operator *(SizeD sz1, double dVal)
    {
        return SizeD.Mul(sz1, dVal);
    }



    public static SizeD operator /(SizeD sz1, double dVal)
    {
        return SizeD.Div(sz1, dVal);
    }



    /// <summary>
    /// Проверяет, действительно ли две структуры <see cref="T:System.Drawing.SizeD"/> эквивалентны.
    /// </summary>
    /// 
    /// <returns>
    /// Этот оператор возвращает значение true, если параметры <paramref name="sz1"/> и <paramref name="sz2"/> имеют равные ширину и высоту; в противном случае возвращается значение false.
    /// </returns>
    /// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора равенства.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора равенства.</param><filterpriority>3</filterpriority>
    public static bool operator ==(SizeD sz1, SizeD sz2)
    {
      if ((double) sz1.Width == (double) sz2.Width)
        return (double) sz1.Height == (double) sz2.Height;
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
    public static bool operator !=(SizeD sz1, SizeD sz2)
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
    public static SizeD Add(SizeD sz1, SizeD sz2)
    {
      return new SizeD(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
    }

    /// <summary>
    /// Вычитает ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> из ширины и высоты другой структуры <see cref="T:System.Drawing.SizeD"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Структура <see cref="T:System.Drawing.SizeD"/>, полученная в результате операции вычитания.
    /// </returns>
    /// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора вычитания.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора вычитания.</param>
    public static SizeD Subtract(SizeD sz1, SizeD sz2)
    {
      return new SizeD(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
    }



    public static SizeD Mul(SizeD sz1, double dVal)
    {
        return new SizeD(sz1.Width * dVal, sz1.Height * dVal);
    }



    public static SizeD Div(SizeD sz1, double dVal)
    {
        if (dVal == 0.0d)
        {
            return new SizeD(double.NaN, double.NaN);
        }
        else if (double.IsNaN(dVal))
        {
            return new SizeD(0.0d, 0.0d);
        }
        else
        {
            return new SizeD(sz1.Width / dVal, sz1.Height / dVal);
        }
    }



    public override bool Equals(object obj)
    {
      if (!(obj is SizeD))
        return false;
      SizeD SizeD = (SizeD) obj;
      if ((double) SizeD.Width == (double) this.Width && (double) SizeD.Height == (double) this.Height)
        return SizeD.GetType().Equals(this.GetType());
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

    /// <summary>
    /// Преобразует структуру <see cref="T:System.Drawing.SizeD"/> в структуру <see cref="T:System.Drawing.PointF"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Возвращает структуру <see cref="T:System.Drawing.PointF"/>.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public PointD ToPointF()
    {
      return (PointD) this;
    }

    /// <summary>
    /// Преобразует структуру <see cref="T:System.Drawing.SizeD"/> в структуру <see cref="T:System.Drawing.Size"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Возвращает структуру <see cref="T:System.Drawing.Size"/>.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public Size ToSize()
    {
        return new Geometry.Size((int)Math.Truncate(width), (int)Math.Truncate(height));
    }



    public SizeF ToSizeF()
    {
        SizeF tmp = new SizeF((float)this.width, (float)this.height);
        return tmp;
    }




      public double DiagonalLength
      {
          get { return Math.Sqrt(width*width + height*height); }
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
      return "{Width=" + this.width.ToString((IFormatProvider) CultureInfo.CurrentCulture) + ", Height=" + this.height.ToString((IFormatProvider) CultureInfo.CurrentCulture) + "}";
    }
  }
}
