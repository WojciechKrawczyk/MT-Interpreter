namespace Interpreter.Modules.SourceCodeReaderModule
{
    public interface ISourceCodeReader
    {
        public char GetNextSymbol();

        public bool HasNextSymbol();
    }
}
