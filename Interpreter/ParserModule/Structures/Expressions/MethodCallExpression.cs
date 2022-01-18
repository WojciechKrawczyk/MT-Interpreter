using Interpreter.ParserModule.Structures.Instructions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class MethodCallExpression : MethodCall, IExpression
    {
        public MethodCallExpression(string objectName, FunctionCall function) : base(objectName, function)
        {
        }

        public new string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitMethodCallExpression(this, scopeContext);
    }
}