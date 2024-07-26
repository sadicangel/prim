using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundStructDeclaration LowerStructDeclaration(BoundStructDeclaration node, LowererContext context)
    {
        var members = LowerList(node.Members, context, LowerMemberDeclaration);
        if (members is null)
            return node;

        return node with { Members = new(members) };

        static BoundMemberDeclaration LowerMemberDeclaration(BoundMemberDeclaration node, LowererContext context)
        {
            return node.BoundKind switch
            {
                BoundKind.PropertyDeclaration => LowerPropertyDeclaration((BoundPropertyDeclaration)node, context),
                BoundKind.MethodDeclaration => LowerMethodDeclaration((BoundMethodDeclaration)node, context),
                BoundKind.OperatorDeclaration => LowerOperatorDeclaration((BoundOperatorDeclaration)node, context),
                BoundKind.ConversionDeclaration => LowerConversionDeclaration((BoundConversionDeclaration)node, context),
                _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'")
            };

            static BoundPropertyDeclaration LowerPropertyDeclaration(BoundPropertyDeclaration node, LowererContext context)
            {
                var expression = LowerExpression(node.Expression, context);
                if (ReferenceEquals(expression, node.Expression))
                    return node;

                return node with { Expression = expression };
            }

            static BoundMethodDeclaration LowerMethodDeclaration(BoundMethodDeclaration node, LowererContext context)
            {
                var body = LowerExpression(node.Body, context);
                if (ReferenceEquals(body, node.Body))
                    return node;

                return node with { Body = body };
            }

            static BoundOperatorDeclaration LowerOperatorDeclaration(BoundOperatorDeclaration node, LowererContext context)
            {
                var body = LowerExpression(node.Body, context);
                if (ReferenceEquals(body, node.Body))
                    return node;

                return node with { Body = body };
            }

            static BoundConversionDeclaration LowerConversionDeclaration(BoundConversionDeclaration node, LowererContext context)
            {
                var body = LowerExpression(node.Body, context);
                if (ReferenceEquals(body, node.Body))
                    return node;

                return node with { Body = body };
            }
        }
    }
}
