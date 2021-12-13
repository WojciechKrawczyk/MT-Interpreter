using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class IfInstruction : Instruction
    {
        public IExpression Condition { get; }
        public IEnumerable<Instruction> BaseInstructions { get; }
        public IEnumerable<Instruction> ElseInstructions { get; }

        public IfInstruction(IExpression condition, IEnumerable<Instruction> baseInstructions, IEnumerable<Instruction> elseInstructions)
        {
            Condition = condition;
            BaseInstructions = baseInstructions;
            ElseInstructions = elseInstructions;
        }
    }
}