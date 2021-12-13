using Interpreter.ParserModule.Structures.Expressions.Types;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class AdditiveExpression : Expression
    {
        public AdditiveExpressionType Type { get; }

        public AdditiveExpression(AdditiveExpressionType type, IExpression left, IExpression right) : base(left, right)
        {
            Type = type;
        }
    }
}