using Interpreter.ParserModule.Types;

namespace Interpreter.ParserModule
{
    public class Parameter
    {
        public Type Type { get; }
        public string Name { get; }

        public Parameter(Type type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}