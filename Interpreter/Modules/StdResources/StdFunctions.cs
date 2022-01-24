using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Definitions;
using Interpreter.Modules.ParserModule.Structures.Instructions;
using Interpreter.Modules.SemanticValidatorModule;
using Interpreter.Modules.SemanticValidatorModule.DefinedStructures;

namespace Interpreter.Modules.StdResources
{
    public static class StdFunctions
    {
        public static readonly Dictionary<string, DefinedFunction> Functions = new()
        {
            {"PrintInt", new DefinedFunction(new FunctionDefinition("PrintInt", "void", new []{new FunctionDefinition.Parameter("int", "parameter")}, new List<IInstruction>()), new ScopeContext())},
            {"PrintBool", new DefinedFunction(new FunctionDefinition("PrintBool", "void", new []{new FunctionDefinition.Parameter("bool", "parameter")}, new List<IInstruction>()), new ScopeContext())},
            {"PrintString", new DefinedFunction(new FunctionDefinition("PrintString", "void", new []{new FunctionDefinition.Parameter("string", "parameter")}, new List<IInstruction>()), new ScopeContext())},
        };
    }
}