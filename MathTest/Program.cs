using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PINQ;

namespace MathTest
{
    class Program
    {
        protected static System.Random random = new System.Random();

       
        private static object Laplace(double stddev)
        {
            double uniform = random.NextDouble() - 0.5;
            Console.WriteLine("uniform: " + uniform);
            var sign = Math.Sign(uniform);
            Console.WriteLine("Sign: " + sign);
            var result = stddev * Math.Sign(uniform) * Math.Log(1 - 2.0 * Math.Abs(uniform));
            return result;
        }

        public static void test1()
        {
            List<int> range = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Project the square of each int value.
            IEnumerable<int> squares =
                range.AsQueryable().Select(x => x * x).Select(x => x * x);

            foreach (int num in squares)
                Console.WriteLine(num);
        }

        static void Main(string[] args)
        {
            //var result = Laplace(2.0);
            //Console.WriteLine("result: " + result);
            test1();
            Console.ReadKey();
        }
    }
}
