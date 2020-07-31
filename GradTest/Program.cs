using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;

namespace GradTest
{
    class Program
    {
        static double[] xArr = {0, 3, 9, 14, 15, 19, 20, 21, 30, 35,
              40, 41, 42, 43, 54, 56, 67, 69, 72, 88 };
        static double[] yArr = {33, 68, 34, 34, 37, 71, 37, 44, 48, 49,
              53, 49, 50, 48, 56, 60, 61, 63, 44, 71};
        static int delta = 2;

        private static int len = xArr.Length;

        static void Main(string[] args)
        {
            VectorVersion();
        }

        static void ScalarVersion()
        {
            //函数只有一个参数时
            var algorithm = new GoldenSectionMinimizer(1e-8, 100000);
            var f1 = new Func<double, double>(ScalarHuberLoss);
            var obj = ObjectiveFunction.ScalarValue(f1);
            var r1 = GoldenSectionMinimizer.Minimum(obj, -10, 10);

            //Console.WriteLine(Math.Abs(r1.MinimizingPoint - 3.0));
            double theta1 = r1.MinimizingPoint;
            Console.WriteLine("theta1 = " + theta1);//最小时的 x 值 -> theta1
            Console.WriteLine("Minimum Loss = " + ScalarHuberLoss(theta1));
            int len = xArr.Length;
            foreach (double x in yArr)
            {
                Console.WriteLine(theta1 * x);
            }
        }

        static double ScalarHuberLoss(double theta1)
        {
            int len = yArr.Length;
            double res = 0;
            double sumLoss = 0;
            for (int i = 0; i < len; i++)
            {
                res = Math.Abs(yArr[i] - theta1 * xArr[i]);
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

        static void VectorVersion()
        {
            //初始化两个矩阵
            var xMat = new DenseMatrix(2, len);
            var yMat = new DenseMatrix(1, len);
            for (var i = 0; i < len; i++)
            {
                xMat[0, i] = 1;
                xMat[1, i] = xArr[i];
            }

            for (var i = 0; i < len; i++)
            {
                yMat[0, i] = yArr[i];
            }

            Vector<double> dVectorHuberLoss(Vector<double> theta)
            {
                /*
                 * input:上一次得到的θ
                 * output:梯度下降后的下一组θ
                 */
                Console.WriteLine("receive argument:{0} {1}", theta[0], theta[1]);
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
                var ret = new DenseVector(new[] {0.01 * ans[0, 0] / len, 0.01 * ans[0, 1] / len});
                ret[0] = -0.01 * ans[0, 0] / len;
                ret[1] = -0.01 * ans[0, 1] / len;
                //Console.WriteLine("next theta step: theta[0] = {0} theta[1] = {1}" , ret[0], ret[1]);
                Console.WriteLine();
                return ret;
            }
            /*
            var numiter = 500;
            var theta = new DenseVector(new []{0.0, 0.0});
            for (int i = 0; i < numiter; i++)
            {
                Console.WriteLine(theta);
                
                Console.WriteLine(theta);
            }*/       
            var solver = new BfgsMinimizer(1e-8, 1e-8, 1e-8, 1000);
            var f = new Func<Vector<double>, double>(VectorHuberLoss);
            var g = new Func<Vector<double>, Vector<double>>(dVectorHuberLoss);
            //dVectorHuberLoss(new DenseVector(new[] {0.038, 1.332}));
        
            var obj = ObjectiveFunction.Gradient(f, g);
            var r1 = solver.FindMinimum(obj, new DenseVector(new[]{0.0, 0.0}));
            Console.WriteLine(r1.MinimizingPoint);
        }

        static double VectorHuberLoss(Vector<double> theta)
        {
            var len = yArr.Length;
            double sumLoss = 0;
            for (var i = 0; i < len; i++)
            {
                var res = Math.Abs(yArr[i] - theta[1] * xArr[i] - theta[0]);
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
    }
}
