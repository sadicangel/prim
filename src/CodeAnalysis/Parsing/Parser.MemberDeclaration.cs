using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Expressions.Declarations;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static MemberDeclarationSyntax ParseMemberDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        return iterator.Current.SyntaxKind switch
        {
            SyntaxKind.ImplicitKeyword or SyntaxKind.ExplicitKeyword => ParseConversionDeclaration(syntaxTree, iterator),
            _ when SyntaxFacts.IsOperator(iterator.Current.SyntaxKind) => ParseOperatorDeclaration(syntaxTree, iterator),
            _ when iterator.Peek(2).SyntaxKind is SyntaxKind.ParenthesisOpenToken => ParseMethodDeclaration(syntaxTree, iterator),
            _ => ParsePropertyDeclaration(syntaxTree, iterator),
        };

        static ConversionDeclarationSyntax ParseConversionDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var conversionKeyword = iterator.Match([SyntaxKind.ImplicitKeyword, SyntaxKind.ExplicitKeyword]);
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(syntaxTree, iterator);
            if (type is not LambdaTypeSyntax functionType)
                throw new InvalidOperationException($"Unexpected type '{type.GetType().Name}'. Expected '{nameof(LambdaTypeSyntax)}'");
            var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
            var body = ParseTerminatedExpression(syntaxTree, iterator);

            return new ConversionDeclarationSyntax(
                syntaxTree,
                conversionKeyword,
                colonToken,
                functionType,
                equalsToken,
                body);
        }

        static OperatorDeclarationSyntax ParseOperatorDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var operatorToken = iterator.Match();
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(syntaxTree, iterator);
            if (type is not LambdaTypeSyntax functionType)
                throw new InvalidOperationException($"Unexpected type '{type.GetType().Name}'. Expected '{nameof(LambdaTypeSyntax)}'");
            var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
            var body = ParseTerminatedExpression(syntaxTree, iterator);

            return new OperatorDeclarationSyntax(
                syntaxTree,
                operatorToken,
                colonToken,
                functionType,
                equalsToken,
                body);
        }

        static MethodDeclarationSyntax ParseMethodDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var name = ParseSimpleNameExpression(syntaxTree, iterator);
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(syntaxTree, iterator);
            if (type is not LambdaTypeSyntax functionType)
                throw new InvalidOperationException($"Unexpected type '{type.GetType().Name}'. Expected '{nameof(LambdaTypeSyntax)}'");
            var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
            var body = ParseTerminatedExpression(syntaxTree, iterator);

            return new MethodDeclarationSyntax(
                syntaxTree,
                name,
                colonToken,
                functionType,
                equalsToken,
                body);
        }

        static PropertyDeclarationSyntax ParsePropertyDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var name = ParseSimpleNameExpression(syntaxTree, iterator);
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(syntaxTree, iterator);
            var initValue = default(InitValueExpressionSyntax);
            var semicolonToken = default(SyntaxToken);
            if (iterator.TryMatch(out var equalsToken, SyntaxKind.EqualsToken))
            {
                var expression = ParseExpression(syntaxTree, iterator);
                initValue = new InitValueExpressionSyntax(syntaxTree, equalsToken, expression);
            }
            if (initValue?.IsTerminated is not true)
                semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);

            return new PropertyDeclarationSyntax(
                syntaxTree,
                name,
                colonToken,
                type,
                initValue,
                semicolonToken);
        }
    }
}
