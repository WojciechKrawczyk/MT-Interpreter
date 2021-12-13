using System.Collections.Generic;
using Interpreter.ParserModule.Structures;
using Interpreter.ParserModule.Structures.Definitions;

namespace Interpreter.ParserModule
{
    public class ProgramInstance
    {
        public Dictionary<string, FunctionDefinition> Functions { get; }
        public Dictionary<string, ClassDefinition> Classes { get; }
        
        public ProgramInstance(Dictionary<string, FunctionDefinition> functions, Dictionary<string, ClassDefinition> classes)
        {
            Functions = functions;
            Classes = classes;
        }
    }
}