using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class ReturnInstruction : Instruction
    {
        public IExpression ToReturn { get; }

        public ReturnInstruction(IExpression toReturn)
        {
            ToReturn = toReturn;
        }
    }
}