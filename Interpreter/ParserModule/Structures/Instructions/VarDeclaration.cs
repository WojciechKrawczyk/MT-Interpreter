using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class VarDeclaration : Instruction
    {
        public string Name { get; }
        public string Type { get; }
        public IExpression Value { get; }

        public VarDeclaration(string name, string type, IExpression value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}