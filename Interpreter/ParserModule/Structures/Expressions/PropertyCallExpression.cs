using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class PropertyCallExpression : IExpression
    {
        public string ObjectName { get; }
        public string PropertyName { get; }

        public PropertyCallExpression(string objectName, string propertyName)
        {
            ObjectName = objectName;
            PropertyName = propertyName;
        }

        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitPropertyCallExpression(this, scopeContext);
    }
}