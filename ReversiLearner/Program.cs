using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReversiLearner
{
    class Program
    {
        static void Main(string[] args)
        {
            //var array = File.ReadAllText(@"ParamsData.txt").Split('\n');
            Console.Write("input the number of matches: ");
            var match = int.Parse(Console.ReadLine().Replace(" ",""));
            Console.Write("input mutation width: ");
            var width = int.Parse(Console.ReadLine().Replace(" ",""));
            Console.Write("input the number of epocs: ");
            var epocs = int.Parse(Console.ReadLine().Replace(" ",""));
            var learner = new Learn(match,width,@"ParamsData.txt");
            //learner.InitParams();
            for (int i = 0; i < epocs; i++)
            {
                var text  = learner.FitParams().Result;
                Console.WriteLine("epoc "+i.ToString() + "::" + text);
            }
            File.WriteAllText(@"ParamsData.txt", string.Join("\r\n", learner.ParamList));
        }

    }
}
