using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Parsing;

namespace CodeAnalysis.Parsing;
internal static partial class Parser
{
    private delegate T ParseNode<T>(SyntaxTree syntaxTree, SyntaxTokenIterator iterator);

    internal static CompilationUnitSyntax Parse(SyntaxTree syntaxTree, bool isScript)
    {
        var tokens = Scanner.Scan(syntaxTree).ToArray();
        if (tokens.Length == 0)
            return new CompilationUnitSyntax(syntaxTree, [], SyntaxFactory.EofToken(syntaxTree));

        ParseNode<ExpressionSyntax> parseExpression = isScript
            ? static (syntaxTree, iterator) => ParseExpression(syntaxTree, iterator, isTerminated: false)
            : ParseDeclaration;

        var iterator = new SyntaxTokenIterator(tokens);

        var expressions = new ReadOnlyList<SyntaxNode>();

        while (iterator.Current.SyntaxKind is not SyntaxKind.EofToken)
        {
            var start = iterator.Current;

            expressions.Add(parseExpression(syntaxTree, iterator));

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (iterator.Current == start)
                _ = iterator.Next();
        }

        var eofToken = iterator.Match(SyntaxKind.EofToken);

        return new CompilationUnitSyntax(syntaxTree, expressions, eofToken);
    }
}
