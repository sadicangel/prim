using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    public static LiteralExpressionSyntax ParseLiteralExpression(
        SyntaxTree syntaxTree,
        SyntaxTokenIterator iterator,
        SyntaxKind syntaxKind,
        PredefinedType type,
        object? value)
    {
        var literalToken = iterator.Next();
        return new LiteralExpressionSyntax(syntaxKind, syntaxTree, literalToken, type, value);
    }
}
