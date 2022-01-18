using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public interface IExpression
    {
        public string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext);
    }
}