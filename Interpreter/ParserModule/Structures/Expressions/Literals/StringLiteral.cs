using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions.Literals
{
    public class StringLiteral : IExpression
    {
        public string Value { get; }

        public StringLiteral(string value)
        {
            Value = value;
        }

        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitStringLiteralExpression(this, scopeContext);
    }
}