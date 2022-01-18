using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions.Literals
{
    public class IntLiteral : IExpression
    {
        public int Value { get; }

        public IntLiteral(int value)
        {
            Value = value;
        }

        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitIntLiteralExpression(this, scopeContext);
    }
}