namespace Interpreter.ParserModule.Structures.Expressions
{
    public interface IOperatorExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }
    }
}