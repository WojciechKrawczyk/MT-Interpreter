using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Instructions
{
    public class Assignment : IInstruction
    {
        public string VariableName { get; }
        public IExpression Expression { get; }
        
        public Assignment(string variableName, IExpression expression)
        {
            VariableName = variableName;
            Expression = expression;
        }

        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitAssignmentInstruction(this, scopeContext);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) => 
            structuresExecutorVisitor.VisitAssignmentInstruction(this, executableScopeContext);
    }
}