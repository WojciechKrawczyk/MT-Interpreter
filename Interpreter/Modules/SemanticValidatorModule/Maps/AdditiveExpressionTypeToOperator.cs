using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Expressions.Types;

namespace Interpreter.Modules.SemanticValidatorModule.Maps
{
    public static class AdditiveExpressionTypeToOperator
    {
        public static readonly Dictionary<AdditiveExpressionType, string> Map = new()
        {
            {AdditiveExpressionType.Plus, "+"},
            {AdditiveExpressionType.Minus, "-"},
        };
    }
}