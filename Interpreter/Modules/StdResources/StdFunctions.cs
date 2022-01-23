using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Definitions;
using Interpreter.Modules.SemanticValidatorModule;
using Interpreter.Modules.SemanticValidatorModule.DefinedStructures;

namespace Interpreter.Modules.StdResources
{
    public static class StdFunctions
    {
        public static Dictionary<string, DefinedFunction> Functions = new()
        {
            {"PrintInt", new DefinedFunction(new FunctionDefinition("PrintInt", "void", new []{new FunctionDefinition.Parameter("int", "parameter")}, null), new ScopeContext())},
            {"PrintBool", new DefinedFunction(new FunctionDefinition("PrintBool", "void", new []{new FunctionDefinition.Parameter("bool", "parameter")}, null), new ScopeContext())}
        };
    }
}