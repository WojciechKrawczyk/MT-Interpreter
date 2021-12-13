using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Instructions;

namespace Interpreter.ParserModule.Structures.Definitions
{
    public class FunctionDefinition : INode
    {
        public string Name { get; }
        public string Type { get; }
        public IEnumerable<Parameter> Parameters { get; }
        public IEnumerable<Instruction> Instructions { get; }
        
        public FunctionDefinition(string name, string type, IEnumerable<Parameter> parameters, IEnumerable<Instruction> instructions)
        {
            Name = name;
            Type = type;
            Parameters = parameters;
            Instructions = instructions;
        }
    }
}