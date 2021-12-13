namespace Interpreter.ParserModule.Structures.Expressions
{
    public class OrExpression : Expression
    {
        public OrExpression(IExpression left, IExpression right) : base(left, right)
        {
        }
    }
}