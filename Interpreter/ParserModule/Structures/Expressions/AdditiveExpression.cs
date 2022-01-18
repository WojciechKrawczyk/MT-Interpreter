using Interpreter.ParserModule.Structures.Expressions.Types;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class AdditiveExpression : IOperatorExpression, IExpression
    {
        public AdditiveExpressionType Type { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }

        public AdditiveExpression(AdditiveExpressionType type, IExpression left, IExpression right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitAdditiveExpression(this, scopeContext);
    }
}