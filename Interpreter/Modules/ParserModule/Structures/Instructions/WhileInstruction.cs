using System.Collections.Generic;
using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Instructions
{
    public class WhileInstruction : IInstruction
    {
        public IExpression Condition { get; }
        public IEnumerable<IInstruction> Instructions { get; }
        
        public WhileInstruction(IExpression condition, IEnumerable<IInstruction> instructions)
        {
            Condition = condition;
            Instructions = instructions;
        }

        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitWhileInstruction(this, scopeContext);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitWhileInstruction(this, executableScopeContext);
    }
}