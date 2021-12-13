using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class PropertyCall : Instruction, IExpression
    {
        public string ObjectName { get; }
        public string PropertyName { get; }

        public PropertyCall(string objectName, string propertyName)
        {
            ObjectName = objectName;
            PropertyName = propertyName;
        }
    }
}