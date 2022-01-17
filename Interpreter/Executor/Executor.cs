using Interpreter.ParserModule.Structures.Definitions;

namespace Interpreter.Executor
{
    public class Executor : IDocumentTreeVisitor
    {
        public void VisitClassDefinition(ClassDefinition classDefinition)
        {
            throw new System.NotImplementedException();
        }

        public void VisitFunctionDefinition(FunctionDefinition functionDefinition)
        {
            throw new System.NotImplementedException();
        }
    }
}