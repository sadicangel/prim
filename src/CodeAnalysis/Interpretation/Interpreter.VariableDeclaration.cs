using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateVariableDeclaration(BoundVariableDeclaration node, Context context)
    {
        var value = EvaluateExpression(node.Expression, context);

        if (value.Type != node.VariableSymbol.Type)
            throw new UnreachableException($"Unexpected expression type '{value.Type.Name}'. Expected '{node.VariableSymbol.Type.Name}'");
        context.EvaluatedScope.Declare(node.VariableSymbol, value);

        return value;
    }
}
