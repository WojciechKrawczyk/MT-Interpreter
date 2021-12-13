using Interpreter.ParserModule.Structures.Expressions.Types;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class MultiplicativeExpression : Expression
    {
        public MultiplicativeExpressionType Type { get; }

        public MultiplicativeExpression(MultiplicativeExpressionType type, IExpression left, IExpression right) : base(left, right)
        {
            Type = type;
        }
    }
}