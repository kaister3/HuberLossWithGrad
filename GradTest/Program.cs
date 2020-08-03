using System;

namespace GradTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var a = new HuberLossSolver();
            var b = a.VectorHuberLossSolver();
            foreach (var d in b)
            {
                Console.WriteLine(d);
            }
        }
    }
}
