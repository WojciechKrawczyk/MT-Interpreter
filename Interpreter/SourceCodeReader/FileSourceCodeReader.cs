namespace Interpreter.SourceCodeReader
{
    public class FileSourceCodeReader : ISourceCodeReader
    {
        private readonly StringSourceCodeReader _sourceCodeReader;

        public FileSourceCodeReader(string path)
        {
            _sourceCodeReader = new StringSourceCodeReader(System.IO.File.ReadAllText(path));
        }

        public char GetNextSymbol() => _sourceCodeReader.GetNextSymbol();

        public bool HasNextSymbol() => _sourceCodeReader.HasNextSymbol();
    }
}