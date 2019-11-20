using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PINQ;
using System.IO;

namespace TestHarness
{
    public class TestHarness
    {
        public class PINQAgentLogger : PINQAgent
        {
            string name;
            double total;
            private double budget = 100;
            public double getBudget() {
                return budget;
            }

            public override bool apply(double epsilon)
            {
                if (epsilon > budget)
                    return false;

                budget -= epsilon;
                total += epsilon;
                //Console.WriteLine("**privacy change**\tdelta: " + epsilon.ToString("0.00") + "\ttotal: " + total.ToString("0.00") + "\t(" + name + ")");
                //Console.WriteLine("**privacy change**\tdelta: {0:F2}\ttotal: {1:F2}\t({2})\tbudget:{3}", epsilon, total, name, budget);

                return true;
            }

            public PINQAgentLogger(string n) { name = n; }
        }

        public static void function1()
        {
            // preparing a private data source
            var filename = @"..\..\test2.txt";
            var data = File.ReadAllLines(filename).AsQueryable();
            PINQAgentLogger agent = new PINQAgentLogger(filename);
            var text = new PINQueryable<string>(data, agent);

            /**** Data is now sealed up. Use from this point on is unrestricted ****/


            // output a noisy count of the number of lines of text
            Console.WriteLine("Lines of text: " + text.count() + "  Lines of text: " + text.NoisyCount(1.0));
            //Console.WriteLine("**privacy change**\tbudget:{0}", agent.getBudget());


            // restrict using a user defined predicate, and count again (with noise)
            Console.WriteLine("Lines with semi-colons: " + text.Where(line => line.Contains(';')).NoisyCount(1.0));
            //Console.WriteLine("**privacy change**\tbudget:{0}", agent.getBudget());

            // think about splitting the records into arrays (declarative, so nothing happens yet)
            var words = text.Select(line => line.Split('*'));
            Console.WriteLine("words: {0}, words_noisy: {1}", words.count(), words.NoisyCount(1.0));

            // partition the data by number of "words", and count how many of each type there are
            var keys = new int[] { 0, 1, 2, 3, 4, 5};
            var parts = words.Partition(keys, line => line.Count());
            foreach (var count in keys)
            {
                Console.WriteLine("");
                Console.WriteLine("Lines with " + count + " words(no noisy):" + "\t" + parts[count].count());
                Console.WriteLine("Lines with " + count + " words(noisy):" + "\t" + parts[count].NoisyCount(1.0));
            }

            Console.ReadKey();
        }

        public static void function2() {
            // preparing a private data source
            var filename = @"..\..\test2.txt";
            var data = File.ReadAllLines(filename).AsQueryable();
            PINQAgentLogger agent = new PINQAgentLogger(filename);
            var text = new PINQueryable<string>(data, agent);

            //测试Select
            Console.WriteLine("测试Select——字符串切割");
            var query1 = data.Select(line => line.Split('*'));
            foreach (var obj in query1)
            {
                Console.WriteLine("*{0}*",obj);
                foreach (var obj2 in obj)
                {
                    Console.WriteLine("-{0}-", obj2);
                }
                Console.WriteLine("----------------");
            }

            //测试where
            Console.WriteLine("测试Where");
            var query2 = data.Where(test => test.Length < 10);
            foreach (var obj in query2)
            {
                Console.WriteLine("{0}", obj);
            }
            Console.ReadKey();
        }

        public static void function3()
        {
            // preparing a private data source
            var filename = @"..\..\test_split.txt";
            var data = File.ReadAllLines(filename).AsQueryable();
            PINQAgentLogger agent = new PINQAgentLogger(filename);
            var text = new PINQueryable<string>(data, agent);

            //测试Select
            Console.WriteLine("测试Select——字符串切割");
            var query1 = data.Select(line => line.Split('\t')).Select(x => new { source = x[0], target = x[1], port = x[2], payload = x[3] });
            foreach (var obj in query1)
            {
                Console.WriteLine("*{0}*", obj);
                //foreach (var obj2 in obj)
                //{
                //    Console.WriteLine("-{0}-", obj2);
                //}
                Console.WriteLine("----------------");
            }

            //测试where
            Console.WriteLine("测试Where");
            var query2 = data.Where(test => test.Length < 10);
            foreach (var obj in query2)
            {
                Console.WriteLine("{0}", obj);
            }
            Console.ReadKey();
        }


        public static void average_test()
        {
            String[] grades = { "10", "20", "30", "40", "50" };

            var grades_int = grades.Select(line => Convert.ToDouble(line));
            var average = grades_int.AsQueryable().Average();
            Console.WriteLine("average: {0}", average);

            Console.ReadKey();
        }

        public static void test3(String[] args)
        {
            // preparing a private data source
            var filename = @"..\..\test3_groupbyname.txt";
            var data = File.ReadAllLines(filename).AsQueryable();
            PINQAgentLogger agent = new PINQAgentLogger(filename);
            var text = new PINQueryable<string>(data, agent);
            var users = text.Select(line => line.Split(','))
                .Where(x => x[1] == args[0])
                .Where(x => x[3] == args[1]);
            Console.WriteLine(" 无噪声——患有癌症且地址为北京的病人有: " + users.count() + "人");
            Console.WriteLine(" 有噪声——患有癌症且地址为北京的病人有: " + users.NoisyCount(10.0) + "人");

            Console.ReadKey();
        }
        public static void test4(String[] args)
        {
            // preparing a private data source
            var filename = @"..\..\test3_groupbyname.txt";
            var data = File.ReadAllLines(filename).AsQueryable();
            PINQAgentLogger agent = new PINQAgentLogger(filename);
            var text = new PINQueryable<string>(data, agent);
            /**
            根据第二列（是否患有癌症）把数据划分成两部分
            数据集中第二列为是否患有癌症，1表示患有癌症，0表示健康
            因此传入的参数 String[] args = {"1", "0"}
            **/
            var parts = text.Select(line => line.Split(','))
                .Partition(args, fields => fields[1]);
            Console.WriteLine("患癌症的人数{0},加入噪声后：{1}", parts["1"].count(), parts["1"].NoisyCount(10.0));
            Console.WriteLine("不患癌症的人数{0},加入噪声后：{1}", parts["0"].count(), parts["0"].NoisyCount(10.0));
            Console.WriteLine();

            Console.WriteLine("【1】代表患病，【0】代表未患病");
            foreach (var query in args)
            {
                //根据地址将数据子集标识
                Console.WriteLine("*************");
                var users = parts[query].GroupBy(fields => fields[3]);

                int[] nums = {1, 2, 3, 4, 5};
                var freqs = users.Partition(nums, group => group.Count());
                
                foreach(var count in nums)
                {
                    Console.WriteLine("患病【" + query + "】人数为" + count + "的地区数量：" 
                        + freqs[count].count() + " 添加噪声后:" + freqs[count].NoisyCount(10.0));
                }

                Console.WriteLine("--------------");
                String[] address = {"beijing", "shanghai", "guangzhou" };
                var details = parts[query].Partition(address, fields => fields[3]);
                Console.WriteLine("北京地区患病【" + query + "】人数{0}，添加噪声后{1}",
                    details["beijing"].count(),details["beijing"].NoisyCount(10.0));
                Console.WriteLine("上海地区患病【" + query + "】人数{0}，添加噪声后{1}",
                    details["shanghai"].count(), details["shanghai"].NoisyCount(10.0));
                Console.WriteLine("广州地区患病【" + query + "】人数{0}，添加噪声后{1}", 
                    details["guangzhou"].count(), details["guangzhou"].NoisyCount(10.0));
                Console.WriteLine();
            }


            Console.ReadKey();
        }
        public static void test5(String[] args)
        {
            // preparing a private data source
            var filename = @"..\..\processed.cleveland.data";
            var data = File.ReadAllLines(filename, Encoding.UTF8).AsQueryable();
            PINQAgentLogger agent = new PINQAgentLogger(filename);
            var text = new PINQueryable<string>(data, agent);

            var parts = text.Select(line => line.Split(','))
                .Partition(args, fields => fields[13]);
            Console.WriteLine("不患心脏病的人数{0},加入噪声后：{1}", parts["0"].count(), parts["0"].NoisyCount(1.0));
            Console.WriteLine("患1病的人数{0},加入噪声后：{1}", parts["1"].count(), parts["1"].NoisyCount(1.0));
            Console.WriteLine("患2病的人数{0},加入噪声后：{1}", parts["2"].count(), parts["2"].NoisyCount(1.0));
            Console.WriteLine("患3病的人数{0},加入噪声后：{1}", parts["3"].count(), parts["3"].NoisyCount(1.0));
            Console.WriteLine("患4病的人数{0},加入噪声后：{1}", parts["4"].count(), parts["4"].NoisyCount(1.0));
            Console.WriteLine("总人数：{0}", parts["0"].count() + parts["1"].count() + parts["2"].count() + parts["3"].count() + parts["4"].count());
            Console.WriteLine();

            Console.ReadKey();
        }

        public static void test6()
        {
            // preparing a private data source
            var filename = @"..\..\test_split.txt";
            var data = File.ReadAllLines(filename).AsQueryable();
            PINQAgentLogger agent = new PINQAgentLogger(filename);
            var text = new PINQueryable<string>(data, agent);

            //测试Select
            Console.WriteLine("测试Select——字符串切割");
            var query1 = data.Select(line => line.Split('\t')).Select(x => new { source = x[0], target = x[1], port = x[2], payload = x[3] });
            foreach (var obj in query1)
            {
                Console.WriteLine("*{0}*", obj);
                //foreach (var obj2 in obj)
                //{
                //    Console.WriteLine("-{0}-", obj2);
                //}
                Console.WriteLine("----------------");
            }

            //测试where
            Console.WriteLine("测试Where");
            var query2 = data.Where(test => test.Length < 10);
            foreach (var obj in query2)
            {
                Console.WriteLine("{0}", obj);
            }
            Console.ReadKey();
        }

        public static void test7()
        {
            // preparing a private data source
            var filename = @"..\..\test_split.txt";
            var data = File.ReadAllLines(filename).AsQueryable();
            PINQAgentLogger agent = new PINQAgentLogger(filename);
            var text = new PINQueryable<string>(data, agent);

            //测试Select
            Console.WriteLine("Iqueryable");
            var query1 = data.Select(line => line.Split('\t')).Select(x => new { source = x[0], target = x[1], port = x[2], payload = x[3] });
            foreach (var obj in query1)
            {
                Console.WriteLine("*{0}*", obj);
                Console.WriteLine("----------------");
            }
            Console.WriteLine("                  ");
            Console.WriteLine("PINQueryable");
            var query2 = text.Select(line => line.Split('\t')).Select(x => new { source = x[0], target = x[1], port = x[2], payload = x[3] });
            foreach (var obj in query2)
            {
                Console.WriteLine("*{0}*", obj);
                Console.WriteLine("----------------");
            }
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            //function1();
            //function2();
            //average_test();
            //function3();

            //string[] s = { "1", "beijing" };
            //test3(s);

            //string[] s2 = { "1", "0" };
            //test4(s2);

            //string[] heart = {"0", "1", "2", "3", "4"};
            //test5(heart);

            //test6();
            test7();
        }
    }
}
