namespace Interpreter.SourceCodeReader
{
    public interface ISourceCodeReader
    {
        public char GetNextSymbol();

        public bool HasNextSymbol();
    }
}
