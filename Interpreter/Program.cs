using System;
using Interpreter.SourceCodeReader;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Type path to .txt file with source code: ");
            var path = Console.ReadLine();
            var read = new FileSourceCodeReader(path);
        }
    }
}