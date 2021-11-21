using System;

namespace Interpreter.SourceCodeReader
{
    public class NoSymbolException : Exception
    {
        public NoSymbolException() : base("No more signs in this code source.") { }
    }
}
