using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class WhileInstruction : Instruction
    {
        public IExpression Condition { get; }
        public IEnumerable<Instruction> Instructions { get; }
        
        public WhileInstruction(IExpression condition, IEnumerable<Instruction> instructions)
        {
            Condition = condition;
            Instructions = instructions;
        }
    }
}