using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Geometry
{
    public class Common
    {
        public static object deg2rad(object phiDeg)
        {
            if (phiDeg is double)
            {
                return Math.PI*(double)phiDeg/180.0d;
            }

            if (phiDeg is double[])
            {
                DenseVector dvPhiDeg = DenseVector.OfEnumerable((double[]) phiDeg);
                return (dvPhiDeg*Math.PI/180.0d).ToArray();
            }

            if (phiDeg is DenseVector)
            {
                return ((DenseVector)phiDeg * Math.PI / 180.0d).ToArray();
            }

            if (phiDeg is Vector<double>)
            {
                return ((DenseVector)phiDeg * Math.PI / 180.0d).ToArray();
            }

            if (phiDeg is IEnumerable<double>)
            {
                return (DenseVector.OfEnumerable((IEnumerable<double>)phiDeg) * Math.PI / 180.0d).ToArray();
            }

            if (phiDeg is double[,])
            {
                DenseMatrix dmPhiDeg = DenseMatrix.OfArray((double[,]) phiDeg);
                return (Math.PI*dmPhiDeg/180.0d).ToArray();
            }

            if (phiDeg is DenseMatrix)
            {
                return (Math.PI * (DenseMatrix)phiDeg / 180.0d).ToArray();
            }


            return null;
        }
    }
}
