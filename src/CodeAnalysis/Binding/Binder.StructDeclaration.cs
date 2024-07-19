using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundStructDeclaration BindStructDeclaration(StructDeclarationSyntax syntax, BinderContext context)
    {
        var symbolName = syntax.Name.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not TypeSymbol typeSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(StructDeclarationSyntax)}'");

        var members = new BoundList<BoundMemberDeclaration>.Builder(syntax.Members.Count);
        foreach (var member in syntax.Members)
        {
            members.Add(member.SyntaxKind switch
            {
                SyntaxKind.PropertyDeclaration =>
                    BindPropertyDeclaration((PropertyDeclarationSyntax)member, typeSymbol, context),
                SyntaxKind.MethodDeclaration =>
                    BindMethodDeclaration((MethodDeclarationSyntax)member, typeSymbol, context),
                SyntaxKind.OperatorDeclaration =>
                    BindOperatorDeclaration((OperatorDeclarationSyntax)member, typeSymbol, context),
                SyntaxKind.ConversionDeclaration =>
                    BindConversionDeclaration((ConversionDeclarationSyntax)member, typeSymbol, context),
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{member.SyntaxKind}'")
            });
        }
        return new BoundStructDeclaration(syntax, typeSymbol, members.ToBoundList());

        static BoundPropertyDeclaration BindPropertyDeclaration(PropertyDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
        {
            var property = typeSymbol.GetProperty(syntax.Name.Text)
                ?? throw new UnreachableException($"Unexpected property '{syntax.Name.Text}'");

            // TODO: Allow init expression to be optional, if property is optional.
            var init = Coerce(BindExpression(syntax.Init, context), property.Type, context);

            return new BoundPropertyDeclaration(syntax, property, init);
        }

        static BoundMethodDeclaration BindMethodDeclaration(MethodDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
        {
            var type = BindLambdaType(syntax.Type, context);
            var method = typeSymbol.GetMethod(syntax.Name.Text, type)
                ?? throw new UnreachableException($"Unexpected method '{syntax.Name.Text}'");

            var body = BindMethodBody(syntax.Body, method, context);

            return new BoundMethodDeclaration(syntax, method, body);
        }

        static BoundOperatorDeclaration BindOperatorDeclaration(OperatorDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
        {
            var type = BindLambdaType(syntax.Type, context);
            var @operator = typeSymbol.Type.GetOperator(syntax.OperatorToken.SyntaxKind, type)
                ?? throw new UnreachableException($"Unexpected operator '{syntax.OperatorToken.Text}'");

            // TODO: Either here or when declaring, must ensure only 1 or 2 parameters.
            var body = BindMethodBody(syntax.Body, @operator, context);

            return new BoundOperatorDeclaration(syntax, @operator, body);
        }

        static BoundConversionDeclaration BindConversionDeclaration(ConversionDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
        {
            var type = BindLambdaType(syntax.Type, context);
            var conversion = typeSymbol.GetConversion(type)
                ?? throw new UnreachableException($"Unexpected conversion '{type}'");

            // TODO: Either here or when declaring, must ensure only 1 parameter.
            var body = BindMethodBody(syntax.Body, conversion, context);

            return new BoundConversionDeclaration(syntax, conversion, body);
        }
    }
}
