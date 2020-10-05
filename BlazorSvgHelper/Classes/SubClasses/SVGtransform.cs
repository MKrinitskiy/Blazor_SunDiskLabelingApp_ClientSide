using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BlazorSvgHelper.Classes.SubClasses
{
    [Serializable]
    public class SVGtransform
    {
        private DenseMatrix TransformMatrix { get; set; }

        public SVGtransform(): this(translateX: 0.0, translateY: 0.0, scaleX: 1.0, scaleY: 1.0, rotateCx: 0.0, rotateCy: 0.0, rotateAngleDegrees: 0.0)
        {

        }
        

        public SVGtransform(double translateX = 0.0, double translateY = 0.0, double scaleX = 1.0, double scaleY = 1.0, double rotateCx = 0.0, double rotateCy = 0.0, double rotateAngleDegrees = 0.0)
        {
            TransformMatrix = DenseMatrix.CreateDiagonal(3, 3, 1.0);

            DenseMatrix translateMatrix = DenseMatrix.OfArray( new double[,]
                {{1,0,translateX},{0,1,translateY},{0,0,1}});

            DenseMatrix scaleMatrix = DenseMatrix.OfArray(new double[,] {{scaleX,0,0}, {0,scaleY,0}, {0,0,1}});

            DenseMatrix rotateMatrix_trans1 = DenseMatrix.OfArray(new double[,]
                {{1,0,-rotateCx},{0,1,-rotateCy},{0,0,1}});
            double a = Math.PI * rotateAngleDegrees / 180.0;
            DenseMatrix rotateMatrix_rotate = DenseMatrix.OfArray(new double[,]
                {{Math.Cos(a), -Math.Sin(a), 0},{Math.Sin(a), Math.Cos(a), 0}, {0,0,1}});
            DenseMatrix rotateMatrix_trans2 = DenseMatrix.OfArray(new double[,]
                {{1,0,rotateCx},{0,1,rotateCy},{0,0,1}});

            TransformMatrix =
                rotateMatrix_trans2.Multiply(rotateMatrix_rotate.Multiply(rotateMatrix_trans1)) as DenseMatrix;
            TransformMatrix = TransformMatrix.Multiply(scaleMatrix) as DenseMatrix;
            TransformMatrix = TransformMatrix.Multiply(translateMatrix) as DenseMatrix;
        }


        public string SVGtransformMatrix => String.Format("{0},{1},{2},{3},{4},{5}", TransformMatrix[0, 0],
            TransformMatrix[1, 0], TransformMatrix[0, 1], TransformMatrix[1, 1], TransformMatrix[0, 2],
            TransformMatrix[1, 2]);


        public string ToString()
        {
            return "matrix(" + SVGtransformMatrix + ")";
        }
    }
}
