using System.Collections.Immutable;
using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundStructDeclaration BindStructDeclaration(StructDeclarationSyntax syntax, Context context)
    {
        var symbolName = syntax.Name.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not StructTypeSymbol structTypeSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(StructDeclarationSyntax)}'");

        var builder = ImmutableArray.CreateBuilder<BoundMemberDeclaration>(syntax.Members.Count);
        foreach (var member in syntax.Members)
        {
            builder.Add(member.SyntaxKind switch
            {
                SyntaxKind.PropertyDeclaration =>
                    BindPropertyDeclaration((PropertyDeclarationSyntax)member, structTypeSymbol, context),
                SyntaxKind.MethodDeclaration =>
                    BindMethodDeclaration((MethodDeclarationSyntax)member, structTypeSymbol, context),
                SyntaxKind.OperatorDeclaration =>
                    BindOperatorDeclaration((OperatorDeclarationSyntax)member, structTypeSymbol, context),
                SyntaxKind.ConversionDeclaration =>
                    BindConversionDeclaration((ConversionDeclarationSyntax)member, structTypeSymbol, context),
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{member.SyntaxKind}'")
            });
        }
        var members = new BoundList<BoundMemberDeclaration>(builder.ToImmutable());
        return new BoundStructDeclaration(syntax, structTypeSymbol, members);

        static BoundPropertyDeclaration BindPropertyDeclaration(PropertyDeclarationSyntax syntax, TypeSymbol typeSymbol, Context context)
        {
            var property = typeSymbol.GetProperty(syntax.Name.Text)
                ?? throw new UnreachableException($"Unexpected property '{syntax.Name.Text}'");

            BoundExpression init;
            if (syntax.InitValue is not null)
            {
                init = Coerce(BindExpression(syntax.InitValue, context), property.Type, context);
            }
            else if (property.Type.IsOption)
            {
                init = Coerce(BoundLiteralExpression.Unit, property.Type, context);
            }
            else
            {
                context.Diagnostics.ReportUninitializedProperty(syntax.Location, property.Name);
                init = new BoundNeverExpression(syntax);
            }

            return new BoundPropertyDeclaration(syntax, property, init);
        }

        static BoundMethodDeclaration BindMethodDeclaration(MethodDeclarationSyntax syntax, TypeSymbol typeSymbol, Context context)
        {
            var type = BindLambdaType(syntax.Type, context);
            var method = typeSymbol.GetMethod(syntax.Name.Text, type)
                ?? throw new UnreachableException($"Unexpected method '{syntax.Name.Text}'");

            var body = BindLambdaBody(method.IsStatic ? null : typeSymbol, method.Parameters, method.ReturnType, syntax.Body, context);

            return new BoundMethodDeclaration(syntax, method, body);
        }

        static BoundOperatorDeclaration BindOperatorDeclaration(OperatorDeclarationSyntax syntax, TypeSymbol typeSymbol, Context context)
        {
            var type = BindLambdaType(syntax.Type, context);
            var @operator = typeSymbol.Type.GetOperator(syntax.OperatorToken.SyntaxKind, type)
                ?? throw new UnreachableException($"Unexpected operator '{syntax.OperatorToken.Text}'");

            var body = BindLambdaBody(null, @operator.Parameters, @operator.ReturnType, syntax.Body, context);

            return new BoundOperatorDeclaration(syntax, @operator, body);
        }

        static BoundConversionDeclaration BindConversionDeclaration(ConversionDeclarationSyntax syntax, TypeSymbol typeSymbol, Context context)
        {
            var type = BindLambdaType(syntax.Type, context);
            var conversion = typeSymbol.GetConversion(type)
                ?? throw new UnreachableException($"Unexpected conversion '{type}'");

            var body = BindLambdaBody(null, [conversion.Parameter], conversion.ReturnType, syntax.Body, context);

            return new BoundConversionDeclaration(syntax, conversion, body);
        }

        static BoundExpression BindLambdaBody(
            TypeSymbol? containingSymbol,
            BoundList<VariableSymbol> parameters,
            TypeSymbol returnType,
            ExpressionSyntax syntax,
            Context context)
        {
            using (context.PushBoundScope())
            {
                if (containingSymbol is not null)
                {
                    _ = context.BoundScope.Declare(VariableSymbol.This(containingSymbol));
                }

                foreach (var parameterSymbol in parameters)
                {
                    // We've already reported redeclarations.
                    _ = context.BoundScope.Declare(parameterSymbol);
                }

                // TODO: Check for unused parameters.
                var body = Coerce(BindExpression(syntax, context), returnType, context);

                return body;
            }
        }
    }
}
