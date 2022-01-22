using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.SourceCodeReader;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Console.Write("Type path to .txt file with source code: ");
            var path = Console.ReadLine();
            var read = new FileSourceCodeReader(path);*/
            var dic = new Dictionary<string, string>
            {
                {"1", "1"}, {"2", "2"}
            };
            var list = dic.Keys.ToList();
            dic.Remove("1");
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            foreach (var key in dic.Keys)
            {
                Console.WriteLine(key);
            }
        }
    }
}