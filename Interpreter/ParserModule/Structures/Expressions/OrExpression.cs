using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class OrExpression : IOperatorExpression, IExpression
    {
        
        public IExpression Left { get; }
        public IExpression Right { get; }

        public OrExpression(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }
        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitOrExpression(this, scopeContext);
    }
}