using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Expressions.Types;

namespace Interpreter.SemanticValidator.Maps
{
    public static class MultiplicativeExpressionTypeToOperator
    {
        public static Dictionary<MultiplicativeExpressionType, string> Map = new()
        {
            {MultiplicativeExpressionType.Multiplication, "*"},
            {MultiplicativeExpressionType.Division, "/"},
            {MultiplicativeExpressionType.Modulo, "%"}
        };
    }
}