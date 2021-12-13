using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures
{
    public class Variable : INode, IExpression
    {
        public string Name { get; }

        public Variable(string name)
        {
            Name = name;
        }
    }
}