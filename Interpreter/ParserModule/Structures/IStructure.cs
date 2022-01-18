using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures
{
    public interface IStructure
    {
        public void Accept(IStructuresVisitor visitor);
    }
}