using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static LambdaValue EvaluateFunctionDeclaration(BoundFunctionDeclaration node, InterpreterContext context)
    {
        // TODO: Can we merge Function and Variable values?
        var lambda = new LambdaValue(
            node.FunctionSymbol.LambdaType,
            FuncFactory.Create(node.FunctionSymbol.LambdaType, node.Body, context));
        context.EvaluatedScope.Declare(node.FunctionSymbol, lambda);
        return lambda;
    }
}
