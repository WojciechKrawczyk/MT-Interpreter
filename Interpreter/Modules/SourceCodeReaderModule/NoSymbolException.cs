using System;

namespace Interpreter.Modules.SourceCodeReaderModule
{
    public class NoSymbolException : Exception
    {
        public NoSymbolException() : base("No more signs in this code source.") { }
    }
}
