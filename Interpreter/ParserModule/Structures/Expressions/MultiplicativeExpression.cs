using Interpreter.ParserModule.Structures.Expressions.Types;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class MultiplicativeExpression : IOperatorExpression, IExpression
    {
        public MultiplicativeExpressionType Type { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }

        public MultiplicativeExpression(MultiplicativeExpressionType type, IExpression left, IExpression right)
        {
            Type = type;
            Left = left;
            Right = right;
        }
        
        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitMultiplicativeExpression(this, scopeContext);
    }
}