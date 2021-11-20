using System;

namespace Interpreter.SourceCodeReader
{
    public class NoSymbolException : Exception
    {
        public NoSymbolException() : base("W źródle kodu programu nie ma już znaków.") { }
    }
}
