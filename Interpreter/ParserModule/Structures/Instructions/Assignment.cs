using Interpreter.Executor;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
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