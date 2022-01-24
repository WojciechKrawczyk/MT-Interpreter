using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Instructions;

namespace Interpreter.Modules.ParserModule.Structures.Definitions
{
    public class ClassDefinition : INode
    {
        public string Name { get; }
        public FunctionDefinition Constructor { get; }
        public List<FunctionDefinition> Functions { get; }
        public List<VarDeclaration> Properties { get; }

        public ClassDefinition(string name, FunctionDefinition constructor, List<FunctionDefinition> functions, List<VarDeclaration> properties)
        {
            Name = name;
            Constructor = constructor;
            Functions = functions;
            Properties = properties;
        }
    }
}