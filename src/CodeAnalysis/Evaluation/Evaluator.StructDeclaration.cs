using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;

partial class Evaluator
{
    private static StructValue EvaluateStructDeclaration(BoundStructDeclaration node, EvaluatorContext context)
    {
        var structValue = new StructValue(node.StructSymbol.Type);
        context.EvaluatedScope.Declare(node.StructSymbol, structValue);
        foreach (var member in node.Members)
        {
            _ = member.BoundKind switch
            {
                BoundKind.PropertyDeclaration =>
                    EvaluatePropertyDeclaration((BoundPropertyDeclaration)member, structValue, context),
                BoundKind.MethodDeclaration =>
                    EvaluateMethodDeclaration((BoundMethodDeclaration)member, structValue, context),
                BoundKind.OperatorDeclaration =>
                    EvaluateOperatorDeclaration((BoundOperatorDeclaration)member, structValue, context),
                BoundKind.ConversionDeclaration =>
                    EvaluateConversionDeclaration((BoundConversionDeclaration)member, structValue, context),
                _ =>
                    throw new UnreachableException($"Unexpected struct member '{member.BoundKind}'")
            };
        }
        return structValue;

        static object EvaluatePropertyDeclaration(BoundPropertyDeclaration member, StructValue structValue, EvaluatorContext context)
        {
            var value = EvaluateExpression(member.Expression, context);
            structValue.SetProperty(member.PropertySymbol, value);
            return 0;
        }

        static object EvaluateMethodDeclaration(BoundMethodDeclaration member, StructValue structValue, EvaluatorContext context)
        {
            _ = member;
            _ = structValue;
            _ = context;
            return 0;
        }

        static object EvaluateOperatorDeclaration(BoundOperatorDeclaration member, StructValue structValue, EvaluatorContext context)
        {
            _ = member;
            _ = structValue;
            _ = context;
            return 0;
        }

        static object EvaluateConversionDeclaration(BoundConversionDeclaration member, StructValue structValue, EvaluatorContext context)
        {
            _ = member;
            _ = structValue;
            _ = context;
            return 0;
        }
    }
}
