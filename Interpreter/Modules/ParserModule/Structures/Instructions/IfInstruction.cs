using System.Collections.Generic;
using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Instructions
{
    public class IfInstruction : IInstruction
    {
        public IExpression Condition { get; }
        public IEnumerable<IInstruction> BaseInstructions { get; }
        public IEnumerable<IInstruction> ElseInstructions { get; }

        public IfInstruction(IExpression condition, IEnumerable<IInstruction> baseInstructions, IEnumerable<IInstruction> elseInstructions)
        {
            Condition = condition;
            BaseInstructions = baseInstructions;
            ElseInstructions = elseInstructions;
        }

        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitIfInstruction(this, scopeContext);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitIfInstruction(this, executableScopeContext);
    }
}