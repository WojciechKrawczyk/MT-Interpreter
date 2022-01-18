using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
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

        public void Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitIfInstruction(this, scopeContext);
    }
}