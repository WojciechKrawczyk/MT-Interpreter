namespace Interpreter.ParserModule.Structures.Expressions
{
    public class NotExpression : Expression
    {
        public bool IsNegated { get; }

        public NotExpression(bool isNegated, IExpression expression) : base(expression, null)
        {
            IsNegated = isNegated;
        }
    }
}