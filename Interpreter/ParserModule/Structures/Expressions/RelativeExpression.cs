using Interpreter.ParserModule.Structures.Expressions.Types;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class RelativeExpression : IOperatorExpression, IExpression
    {
        public RelativeExpressionType Type { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }

        public RelativeExpression(RelativeExpressionType type, IExpression left, IExpression right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitRelativeExpression(this, scopeContext);
    }
}