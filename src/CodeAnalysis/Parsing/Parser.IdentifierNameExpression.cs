using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static IdentifierNameExpressionSyntax ParseIdentifierNameExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var identifierToken = SyntaxFacts.IsPredefinedType(iterator.Current.SyntaxKind)
            ? iterator.Match()
            : iterator.Match(SyntaxKind.IdentifierToken);
        return new IdentifierNameExpressionSyntax(syntaxTree, identifierToken);
    }
}
