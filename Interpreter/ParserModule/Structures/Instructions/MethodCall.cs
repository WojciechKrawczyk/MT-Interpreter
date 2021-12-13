using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class MethodCall : Instruction, IExpression
    {
        public string ObjectName { get; }
        public FunctionCall Function { get; }

        public MethodCall(string objectName, FunctionCall function)
        {
            ObjectName = objectName;
            Function = function;
        }
    }
}