using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;

namespace GradTest
{
    public class HuberLossSolver
    {
        private readonly double[] _xArr;
        private readonly double[] _yArr;
        /*
         * 在字段声明中，readonly 指示只能在声明期间或在同一个类的构造函数中向字段赋值。
         * 可以在字段声明和构造函数中多次分配和重新分配只读字段
         */
        private const int delta = 3;
        private double[] scalarPred;
        private double[] vectorPred;

        public HuberLossSolver(string filepath)
        {
            var reader = new StreamReader(filepath);
            var x = new List<double>();
            var y = new List<double>();
            var index = 0;
            while (!reader.EndOfStream && index < 50)
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
        /// 只有一个参数时的拟合方法
        /// </summary>
        public double[] ScalarHuberLossSolver()
        {
            var f1 = new Func<double, double>(ScalarHuberLoss);
            var obj = ObjectiveFunction.ScalarValue(f1);
            var r1 = GoldenSectionMinimizer.Minimum(obj, -10, 10);
            var theta1 = r1.MinimizingPoint;
            Console.WriteLine("theta1 = " + theta1);//最小时的 x 值 -> theta1
            Console.WriteLine("Minimum Loss = " + ScalarHuberLoss(theta1));
            scalarPred = new double[_xArr.Length];
            for (var i = 0; i < _xArr.Length; i++)
            {
                //输出预测的y值
                scalarPred[i] = theta1 * _xArr[i];
            }

            return scalarPred;
        }
        
        /// <summary>
        /// 只有一个参数时的 HuberLoss
        /// </summary>
        /// <param name="theta1">回归直线系数</param>
        /// <param name="xArr">x轴 数据</param>
        /// <param name="yArr">y轴 数据</param>
        /// <param name="delta">常数 delta</param>
        /// <returns></returns>
        private double ScalarHuberLoss(double theta1)
        {
            
            var len = _yArr.Length;
            double sumLoss = 0;
            for (var i = 0; i < len; i++)
            {
                var res = Math.Abs(_yArr[i] - theta1 * _xArr[i]);
                // sumLoss += (res <= delta) * res * res / 2 + delta * (res > delta) * (res - delta / 2);
                if (res <= delta)
                {
                    sumLoss += res * res / 2;
                }
                else
                {
                    sumLoss += delta * (res - delta) / 2;
                }
            }
            return sumLoss;
        }
        
        /// <summary>
        /// 输入本次预测的回归系数，输出当前点的梯度（偏导数）
        /// </summary>
        /// <param name="theta"></param>
        /// <returns></returns>
        
        public double[] VectorHuberLossSolver()
        {
            var len = _xArr.Length;
            //初始化两个矩阵
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
            
            Vector<double> dVectorHuberLoss(Vector<double> theta)
            {
                //Console.WriteLine("receive argument:{0} {1}", theta[0], theta[1]);
                var thetaMatrix = new DenseMatrix(1, 2) {[0, 0] = theta[0], [0, 1] = theta[1]};
                var mask = yMat - thetaMatrix * xMat;
                for (var i = 0; i < len; i++)
                {
                    if (mask[0, i] < -delta)
                    {
                        mask[0, i] = -delta;
                    }
                    else if (mask[0, i] > delta)
                    {
                        mask[0, i] = delta;
                    }
                }
                //Console.WriteLine("After:" + mask);

                var ans = mask * xMat.Transpose();
                var ret = new DenseVector(new[] {-ans[0, 0] / len, -ans[0, 1] / len});
                //ret[0] = - ans[0, 0] / len;
                //ret[1] = -ans[0, 1] / len;
                //Console.WriteLine("next theta step: theta[0] = {0} theta[1] = {1}" , ret[0], ret[1]);
                return ret;
            }
            
            var solver = new BfgsMinimizer(1e-8, 1e-8, 1e-8, 1000000);
            var f = new Func<Vector<double>, double>(VectorHuberLoss);
            var g = new Func<Vector<double>, Vector<double>>(dVectorHuberLoss);
            var obj = ObjectiveFunction.Gradient(f, g);
            var r1 = solver.FindMinimum(obj, new DenseVector(new[]{0.0, 0.0}));
            Console.WriteLine(r1.MinimizingPoint.ToString("#0.00000000"));
            // 由于只默认显示1位小数，导致与python计算结果比较时出现十分整齐的误差，格式化之后解决
            vectorPred = new double[len];
            return vectorPred;
        }
        
        /// <summary>
        /// 计算当有两个参数时的HuberLoss
        /// </summary>
        /// <param name="theta"></param>
        /// <returns></returns>
        private double VectorHuberLoss(Vector<double> theta)
        {
            var len = _yArr.Length;
            double sumLoss = 0;
            for (var i = 0; i < len; i++)
            {
                var res = Math.Abs(_yArr[i] - theta[1] * _xArr[i] - theta[0]);
                // sumLoss += (res <= delta) * res * res / 2 + delta * (res > delta) * (res - delta / 2);
                if (res <= delta)
                {
                    sumLoss += res * res / 2;
                }
                else
                {
                    sumLoss += delta * (res - delta / 2);
                }
            }
            return sumLoss;
        }

        public double[] getX()
        {
            return _xArr;
        }

        public double[] getY()
        {
            return _yArr;
        }
    }
}