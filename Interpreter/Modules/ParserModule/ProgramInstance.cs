using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Definitions;

namespace Interpreter.Modules.ParserModule
{
    public class ProgramInstance
    {
        public List<FunctionDefinition> Functions { get; }
        public List<ClassDefinition> Classes { get; }
        
        public ProgramInstance(List<FunctionDefinition> functions, List<ClassDefinition> classes)
        {
            Functions = functions;
            Classes = classes;
        }
    }
}