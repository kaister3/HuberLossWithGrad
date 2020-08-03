using System;

namespace GradTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var filepath = args[0];
            Console.WriteLine(filepath);
            var a = new HuberLossSolver(filepath);
            var b = a.VectorHuberLossSolver();
            foreach (var d in b)
            {
                Console.WriteLine(d);
            }
        }
    }
}
