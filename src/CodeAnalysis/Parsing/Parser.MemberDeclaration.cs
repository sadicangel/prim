using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Operators;
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
            _ when iterator.Peek(1).SyntaxKind is SyntaxKind.ParenthesisOpenToken => ParseMethodDeclaration(syntaxTree, iterator),
            _ => ParsePropertyDeclaration(syntaxTree, iterator),
        };

        static ConversionDeclarationSyntax ParseConversionDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var conversionKeyword = iterator.Match([SyntaxKind.ImplicitKeyword, SyntaxKind.ExplicitKeyword]);
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(syntaxTree, iterator);
            if (type is not FunctionTypeSyntax functionType)
                throw new InvalidOperationException($"Unexpected type '{type.GetType().Name}'. Expected '{nameof(FunctionTypeSyntax)}'");
            var equalsToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
            var body = ParseTerminatedExpression(syntaxTree, iterator);

            // TODO: Validate that function only has 1 parameter.

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
            if (type is not FunctionTypeSyntax functionType)
                throw new InvalidOperationException($"Unexpected type '{type.GetType().Name}'. Expected '{nameof(FunctionTypeSyntax)}'");
            var equalsToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
            var body = ParseTerminatedExpression(syntaxTree, iterator);

            // TODO: Validate that function only has 1 or 2 parameters.

            var (operatorKind, operatorPrecedence) = functionType.Parameters.Count == 1
                ? SyntaxFacts.GetUnaryOperatorPrecedence(operatorToken.SyntaxKind)
                : SyntaxFacts.GetBinaryOperatorPrecedence(operatorToken.SyntaxKind);

            return new OperatorDeclarationSyntax(
                syntaxTree,
                new OperatorSyntax(operatorKind, syntaxTree, operatorToken, operatorPrecedence),
                colonToken,
                functionType,
                equalsToken,
                body);
        }

        static MethodDeclarationSyntax ParseMethodDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(syntaxTree, iterator);
            if (type is not FunctionTypeSyntax functionType)
                throw new InvalidOperationException($"Unexpected type '{type.GetType().Name}'. Expected '{nameof(FunctionTypeSyntax)}'");
            var operatorToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
            var body = ParseTerminatedExpression(syntaxTree, iterator);

            return new MethodDeclarationSyntax(
                syntaxTree,
                identifierToken,
                colonToken,
                functionType,
                operatorToken,
                body);
        }

        static PropertyDeclarationSyntax ParsePropertyDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(syntaxTree, iterator);
            var operatorToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
            var expression = ParseTerminatedExpression(syntaxTree, iterator);

            return new PropertyDeclarationSyntax(
                syntaxTree,
                identifierToken,
                colonToken,
                type,
                operatorToken,
                expression);
        }
    }
}
