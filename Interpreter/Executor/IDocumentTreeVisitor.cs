using Interpreter.ParserModule.Structures.Definitions;

namespace Interpreter.Executor
{
    public interface IDocumentTreeVisitor
    {
        public void VisitClassDefinition(ClassDefinition classDefinition);
        public void VisitFunctionDefinition(FunctionDefinition functionDefinition);
    }
}