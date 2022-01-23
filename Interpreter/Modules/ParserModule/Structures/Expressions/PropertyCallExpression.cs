using Interpreter.Executor;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions
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

        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitPropertyCallExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitPropertyCallExpression(this, executableScopeContext);
    }
}