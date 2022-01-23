using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Definitions;
using Interpreter.Modules.ParserModule.Structures.Instructions;

namespace Interpreter.Modules.SemanticValidatorModule.ValidStructures
{
    public class ValidFunction : FunctionDefinition
    {
        public ValidFunction(string name, string type, IEnumerable<Parameter> parameters, IEnumerable<IInstruction> instructions) : 
            base(name, type, parameters, instructions)
        {
        }
    }
}