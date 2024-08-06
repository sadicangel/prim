using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateVariableDeclaration(BoundVariableDeclaration node, Context context)
    {
        var value = node.Type is LambdaTypeSymbol lambdaType
            ? new LambdaValue(lambdaType, FuncFactory.Create(lambdaType, node.Expression, context))
            : EvaluateExpression(node.Expression, context);

        if (value.Type != node.VariableSymbol.Type)
            throw new UnreachableException($"Unexpected expression type '{value.Type.Name}'. Expected '{node.VariableSymbol.Type.Name}'");
        context.EvaluatedScope.Declare(node.VariableSymbol, value);

        return value;
    }
}
