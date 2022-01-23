using System;
using Interpreter.SourceCodeReader;

namespace Interpreter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.Write("Type path to .txt file with source code: ");
            var path = Console.ReadLine();
            var read = new FileSourceCodeReader(path);
            Interpreter.InterpretProgram(read);
        }
    }
}