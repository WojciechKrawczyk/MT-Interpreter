using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Instructions;

namespace Interpreter.ParserModule.Structures.Definitions
{
    public class FunctionDefinition : INode
    {
        public class Parameter
        {
            public string Type { get; }
            public string Name { get; }

            public Parameter(string type, string name)
            {
                Type = type;
                Name = name;
            }
        }
        public string Name { get; }
        public string Type { get; }
        public IEnumerable<Parameter> Parameters { get; }
        public IEnumerable<IInstruction> Instructions { get; }
        
        public FunctionDefinition(string name, string type, IEnumerable<Parameter> parameters, IEnumerable<IInstruction> instructions)
        {
            Name = name;
            Type = type;
            Parameters = parameters;
            Instructions = instructions;
        }
    }
}