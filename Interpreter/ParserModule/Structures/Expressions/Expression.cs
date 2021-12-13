namespace Interpreter.ParserModule.Structures.Expressions
{
    public class Expression : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public Expression(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }
    }
}