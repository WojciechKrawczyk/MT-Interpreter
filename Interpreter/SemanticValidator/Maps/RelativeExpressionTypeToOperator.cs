using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Expressions.Types;

namespace Interpreter.SemanticValidator.Maps
{
    public static class RelativeExpressionTypeToOperator
    {
        public static Dictionary<RelativeExpressionType, string> Map = new()
        {
            {RelativeExpressionType.Less, "<"},
            {RelativeExpressionType.LessOrEqual, "<="},
            {RelativeExpressionType.Grater, ">"},
            {RelativeExpressionType.GraterOrEqual, ">="},
            {RelativeExpressionType.NotEqual, "!="},
            {RelativeExpressionType.Equal, "=="}
        };
    }
}