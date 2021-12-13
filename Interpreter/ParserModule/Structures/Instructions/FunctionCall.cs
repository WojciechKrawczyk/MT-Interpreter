using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class FunctionCall : Instruction
    {
        public string Name { get; }
        public IEnumerable<IExpression> Arguments { get; }

        public FunctionCall(string name, IEnumerable<IExpression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}