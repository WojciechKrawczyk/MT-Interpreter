namespace Interpreter.ParserModule.Structures.Expressions
{
    public class AndExpression : Expression
    {
        public AndExpression(IExpression left, IExpression right) : base(left, right)
        {
        }
    }
}