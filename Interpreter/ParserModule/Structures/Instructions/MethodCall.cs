using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class MethodCall : IInstruction
    {
        public string ObjectName { get; }
        public FunctionCall Function { get; }

        public MethodCall(string objectName, FunctionCall function)
        {
            ObjectName = objectName;
            Function = function;
        }

        public void Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitMethodCallInstruction(this, scopeContext);
    }
}