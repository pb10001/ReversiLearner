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
            var learner = new Learn(@"ParamsData.txt");
            //learner.InitParams();
            for (int i = 0; i < 10; i++)
            {
                var text  = learner.FitParams().Result;
                Console.WriteLine("epoc "+i.ToString() + "::" + text);
            }
            File.WriteAllText(@"ParamsData.txt", string.Join("\r\n", learner.ParamList));
        }

    }
}
