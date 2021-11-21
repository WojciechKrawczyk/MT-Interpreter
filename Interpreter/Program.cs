using Interpreter.Lexers;
using Interpreter.SourceCodeReader;
using System;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            StringSourceCodeReader reader = new StringSourceCodeReader("program ");
            Lexer lexer = new Lexer(reader);
            var x = lexer.GetNextToken().TokenType;
            Console.WriteLine(char.IsLetterOrDigit('3'));
        }
    }
}