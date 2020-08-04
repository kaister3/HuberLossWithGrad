using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;

namespace GradTest
{
    public static class Outliers
    {
        //寻找离群点的方法
        public static List<int> find(string filepath, Vector<double> theta)
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
            var len = y.Count;
            var res = new double[len];
            double r = 0;
            Console.WriteLine(res.Length);
            Console.WriteLine(x.Count);
            for (var i = 0; i < len; i++)
            {
                res[i] += Math.Abs(y[i] - theta[0] - theta[1] * x[i]);
            }

            var ans = new List<int>();
            var a = res.Sum() / len;
            for (int i = 0; i < len; i++)
            {
                var temp = (res[i] - a) / a;
                if (Math.Abs(temp) > 3)
                {
                    Console.WriteLine("Error : " + y[i]);
                    ans.Append(i);
                }
            }

            return ans;
        }
    }
}