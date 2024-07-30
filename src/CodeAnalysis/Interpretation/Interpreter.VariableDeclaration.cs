using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateVariableDeclaration(BoundVariableDeclaration node, Context context)
    {
        var value = ValueFactory.Create(node.Type, node.Expression, context);
        //var value = node.VariableSymbol.Type is LambdaTypeSymbol lambdaType
        //    ? new LambdaValue(lambdaType, FuncFactory.Create(lambdaType, node.Expression, context))
        //    : EvaluateExpression(node.Expression, context);
        context.EvaluatedScope.Declare(node.VariableSymbol, value);

        return value;
    }
}
