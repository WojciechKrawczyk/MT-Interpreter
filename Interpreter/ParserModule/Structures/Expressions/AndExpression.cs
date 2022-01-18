using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class AndExpression : IOperatorExpression, IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public AndExpression(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }
        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitAndExpression(this, scopeContext);
    }
}