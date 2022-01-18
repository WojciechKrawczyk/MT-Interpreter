using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class VariableExpression : INode, IExpression
    {
        public string Name { get; }

        public VariableExpression(string name)
        {
            Name = name;
        }

        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitVariableExpression(this, scopeContext);
    }
}