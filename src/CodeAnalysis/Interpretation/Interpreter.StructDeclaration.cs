using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;

partial class Interpreter
{
    private static StructValue EvaluateStructDeclaration(BoundStructDeclaration node, Context context)
    {
        var structValue = new StructValue(node.StructTypeSymbol, node.Type);
        context.Module.Declare(node.StructTypeSymbol, structValue);
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

        static object EvaluatePropertyDeclaration(BoundPropertyDeclaration member, StructValue structValue, Context context)
        {
            var value = EvaluateExpression(member.Expression, context);
            structValue.Add(member.PropertySymbol, value);
            return 0;
        }

        static object EvaluateMethodDeclaration(BoundMethodDeclaration member, StructValue structValue, Context context)
        {
            var lambda = new LambdaValue(
                member.MethodSymbol.LambdaType,
                FuncFactory.Create(member.MethodSymbol.LambdaType, member.Body, context));
            structValue.Add(member.MethodSymbol, lambda);
            return 0;
        }

        static object EvaluateOperatorDeclaration(BoundOperatorDeclaration member, StructValue structValue, Context context)
        {
            _ = member;
            _ = structValue;
            _ = context;
            return 0;
        }

        static object EvaluateConversionDeclaration(BoundConversionDeclaration member, StructValue structValue, Context context)
        {
            var lambda = new LambdaValue(
                member.ConversionSymbol.LambdaType,
                FuncFactory.Create(member.ConversionSymbol.LambdaType, member.Body, context));
            structValue.Add(member.ConversionSymbol, lambda);
            return 0;
        }
    }
}
