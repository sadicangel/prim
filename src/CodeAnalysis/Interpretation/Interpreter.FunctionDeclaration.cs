using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static FunctionValue EvaluateFunctionDeclaration(BoundFunctionDeclaration node, InterpreterContext context)
    {
        // TODO: Can we merge Function and Variable values?
        var functionValue = new FunctionValue(node.NameSymbol.Type, FuncFactory.Create(node.NameSymbol, node.Body, context));
        functionValue.SetOperator(node.OperatorSymbol, functionValue);
        context.EvaluatedScope.Declare(node.NameSymbol, functionValue);
        return functionValue;
    }
}
