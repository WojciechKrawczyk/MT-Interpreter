using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions
{
    public class VariableExpression : INode, IExpression
    {
        public string Name { get; }

        public VariableExpression(string name)
        {
            Name = name;
        }

        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitVariableExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitVariableExpression(this, executableScopeContext);
    }
}