namespace Interpreter.Extensions
{
    public static class CharExtensions
    {
        public static bool IsEndOfLine(this char symbol) 
            => symbol is '\n' or '\r';
    }
}