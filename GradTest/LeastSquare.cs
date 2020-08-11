using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;

namespace GradTest
{
    /// <summary>
    /// 最小均方误差的梯度下降法，作为参照
    /// </summary>
    public class LeastSquare
    {
        private readonly double[] _xArr;
        private readonly double[] _yArr;
        /*
         * 在字段声明中，readonly 指示只能在声明期间或在同一个类的构造函数中向字段赋值。
         * 可以在字段声明和构造函数中多次分配和重新分配只读字段
         */
        private double[] scalarPred;
        private double[] vectorPred;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filepath"></param>
        public LeastSquare(string filepath)
        {
            var reader = new StreamReader(filepath);
            var x = new List<double>();
            var y = new List<double>();
            var index = 0;
            while (!reader.EndOfStream && index < 1000)
            {
                var line = reader.ReadLine();
                //Console.WriteLine(line);
                var values = line.Split(',');
                x.Add(double.Parse(values[0]));
                y.Add(double.Parse(values[1]));
                index++;
            }
            Console.WriteLine("Construct Success!");
            _xArr = x.ToArray();
            _yArr = y.ToArray();
        }

        /// <summary>
        /// 运用梯度下降法解决问题
        /// </summary>
        /// <returns></returns>
        public void LeastSquareSolver()
        {
            var len = _xArr.Length;
            var xMat = new DenseMatrix(2, len);
            var yMat = new DenseMatrix(1, len);
            for (var i = 0; i < len; i++)
            {
                xMat[0, i] = 1;
                xMat[1, i] = _xArr[i];
            }

            for (var i = 0; i < len; i++)
            {
                yMat[0, i] = _yArr[i];
            }
            
            Vector<double> dLeastSquareLoss(Vector<double> theta)
            {
                /*
                 * 
                 */
                var theta0 = 0.0;
                var theta1 = 0.0;
                var sum = 0.0;
                //输入猜测的theta，输出当前点的斜率
                for (var i = 0; i < len; i++)
                {
                    var y_pred = theta[0] + theta[1] * _xArr[i];
                    theta0 += y_pred - _yArr[i];
                    theta1 += (y_pred - _yArr[i]) * _xArr[i];
                }
                return new DenseVector(new[] {theta0 / len, theta1 / len});
            }
            
            var solver = new BfgsMinimizer(1e-8, 1e-8, 1e-8, 10000);
            var f = new Func<Vector<double>, double>(LeastSquareLoss);
            var g = new Func<Vector<double>,Vector<double>>(dLeastSquareLoss);
            var obj = ObjectiveFunction.Gradient(f, g);
            var r1 = solver.FindMinimum(obj, new DenseVector(new [] {0.0, 0.0}));
            Console.WriteLine(r1.MinimizingPoint);
        }

        private double LeastSquareLoss(Vector<double> theta)
        {
            return _xArr.Select((t, i) => Math.Pow(theta[0] + theta[1] * t - _yArr[i], 2)).Sum();
        }
    }
}