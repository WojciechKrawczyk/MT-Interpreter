using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions.Literals
{
    public class BoolLiteral : IExpression
    {
        public bool Value { get; }

        public BoolLiteral(bool value)
        {
            Value = value;
        }

        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitBoolLiteralExpression(this, scopeContext);
    }
}