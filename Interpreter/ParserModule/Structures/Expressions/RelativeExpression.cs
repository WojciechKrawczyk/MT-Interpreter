using Interpreter.ParserModule.Structures.Expressions.Types;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class RelativeExpression : Expression
    {
        public RelativeExpressionType Type { get; }

        public RelativeExpression(RelativeExpressionType type, IExpression left, IExpression right) : base(left, right)
        {
            Type = type;
        }
    }
}