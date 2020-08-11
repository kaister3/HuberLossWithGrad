namespace GradTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var path2 = "/home/wyk/RiderProjects/HuberLossWithGrad/GradTest/hxjydq20200203.csv";
            //var b = new HuberLossSolver(args[0]);
            var b = new LeastSquare(args[0]);
            //var theta = b.VectorHuberLossSolver();
            b.LeastSquareSolver();
            
            //var r = Outliers.find(args[0], theta);
            /*foreach (var t in r)
            {
                Console.WriteLine(t);
            }*/
            
            //寻找离群点之后，可以将该点去除后进行再一次的回归
        }
    }
}
