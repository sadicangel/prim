using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static QualifiedNameSyntax ParseQualifiedName(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var left = ParseFirstQualifiedName(syntaxTree, iterator);
        while (iterator.Current.SyntaxKind is SyntaxKind.ColonColonToken)
        {
            var colonColonToken = iterator.Match(SyntaxKind.ColonColonToken);
            var right = ParseSimpleName(syntaxTree, iterator);

            left = new QualifiedNameSyntax(syntaxTree, left, colonColonToken, right);
        }
        return left;

        static QualifiedNameSyntax ParseFirstQualifiedName(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var left = ParseSimpleName(syntaxTree, iterator);
            var colonColonToken = iterator.Match(SyntaxKind.ColonColonToken);
            var right = ParseSimpleName(syntaxTree, iterator);

            return new QualifiedNameSyntax(syntaxTree, left, colonColonToken, right);
        }
    }
}
