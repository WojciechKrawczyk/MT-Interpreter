using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Instructions;

namespace Interpreter.ParserModule.Structures.Definitions
{
    public class ClassDefinition : INode
    {
        public string Name { get; }
        public FunctionDefinition Constructor { get; }
        public Dictionary<string, FunctionDefinition> Functions { get; }
        public Dictionary<string, VarDeclaration> Properties { get; }

        public ClassDefinition(string name, FunctionDefinition constructor, Dictionary<string, FunctionDefinition> functions, Dictionary<string, VarDeclaration> properties)
        {
            Name = name;
            Constructor = constructor;
            Functions = functions;
            Properties = properties;
        }
    }
}