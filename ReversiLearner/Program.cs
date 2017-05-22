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
            var learner = new Learn();
            learner.InitParams();
            var res =learner.EvaluateParams().Result;
            File.WriteAllText(@"ParamsData.txt",string.Join("\r\n",learner.ParamList));
        }

    }
}
