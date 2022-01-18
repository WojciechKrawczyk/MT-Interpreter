using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public interface IInstruction : INode
    {
        public void Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext);
    }
}