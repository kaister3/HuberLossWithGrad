using System;

namespace GradTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var path2 = "/home/wyk/RiderProjects/HuberLossWithGrad/GradTest/hxjydq20200203.csv";
            //var a = new LeastSquare(args[0]);
            var b = new HuberLossSolver(args[0]);
            //a.LeastSquareSolver();
            b.VectorHuberLossSolver();
            //a.LeastSquareSolver();
        }
    }
}
