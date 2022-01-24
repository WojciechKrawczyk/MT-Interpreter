using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Definitions;
using Interpreter.Modules.ParserModule.Structures.Instructions;

namespace Interpreter.Modules.SemanticValidatorModule.ValidStructures
{
    public class ValidClass
    {
        public string Name { get; init; }
        public FunctionDefinition Constructor { get; init; }
        public Dictionary<string, ValidFunction> Methods { get; init; }
        public Dictionary<string, VarDeclaration> Properties { get; init; }
    }
}