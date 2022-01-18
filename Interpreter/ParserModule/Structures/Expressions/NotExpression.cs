using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class NotExpression : IOperatorExpression, IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public NotExpression(IExpression expression)
        {
            Left = expression;
            Right = null;
        }
        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitNotExpression(this, scopeContext);
    }
}