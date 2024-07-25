using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundStructDeclaration LowerStructDeclaration(BoundStructDeclaration node)
    {
        var members = LowerList(node.Members, LowerMemberDeclaration);
        if (members is null)
            return node;

        return node with { Members = new(members) };

        static BoundMemberDeclaration LowerMemberDeclaration(BoundMemberDeclaration node)
        {
            return node.BoundKind switch
            {
                BoundKind.PropertyDeclaration => LowerPropertyDeclaration((BoundPropertyDeclaration)node),
                BoundKind.MethodDeclaration => LowerMethodDeclaration((BoundMethodDeclaration)node),
                BoundKind.OperatorDeclaration => LowerOperatorDeclaration((BoundOperatorDeclaration)node),
                BoundKind.ConversionDeclaration => LowerConversionDeclaration((BoundConversionDeclaration)node),
                _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'")
            };

            static BoundPropertyDeclaration LowerPropertyDeclaration(BoundPropertyDeclaration node)
            {
                var expression = LowerExpression(node.Expression);
                if (ReferenceEquals(expression, node.Expression))
                    return node;

                return node with { Expression = expression };
            }

            static BoundMethodDeclaration LowerMethodDeclaration(BoundMethodDeclaration node)
            {
                var body = LowerExpression(node.Body);
                if (ReferenceEquals(body, node.Body))
                    return node;

                return node with { Body = body };
            }

            static BoundOperatorDeclaration LowerOperatorDeclaration(BoundOperatorDeclaration node)
            {
                var body = LowerExpression(node.Body);
                if (ReferenceEquals(body, node.Body))
                    return node;

                return node with { Body = body };
            }

            static BoundConversionDeclaration LowerConversionDeclaration(BoundConversionDeclaration node)
            {
                var body = LowerExpression(node.Body);
                if (ReferenceEquals(body, node.Body))
                    return node;

                return node with { Body = body };
            }
        }
    }
}
