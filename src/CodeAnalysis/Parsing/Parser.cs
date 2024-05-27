using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Parsing;

namespace CodeAnalysis.Parsing;
internal static partial class Parser
{
    private delegate T ParseNode<out T>(SyntaxTree syntaxTree, SyntaxTokenIterator iterator) where T : SyntaxNode;

    internal static CompilationUnitSyntax Parse(SyntaxTree syntaxTree, bool isScript)
    {
        var tokens = Scanner.Scan(syntaxTree).ToArray();
        if (tokens.Length == 0)
            return new CompilationUnitSyntax(syntaxTree, [], SyntaxFactory.EofToken(syntaxTree));

        ParseNode<ExpressionSyntax> parseExpression = isScript
            ? static (syntaxTree, iterator) => ParseExpression(syntaxTree, iterator, isTerminated: false)
            : ParseDeclaration;

        var iterator = new SyntaxTokenIterator(tokens);

        var expressions = ParseSyntaxList<SyntaxNode>(syntaxTree, iterator, [SyntaxKind.EofToken], parseExpression);

        var eofToken = iterator.Match(SyntaxKind.EofToken);

        return new CompilationUnitSyntax(syntaxTree, expressions, eofToken);
    }
}
