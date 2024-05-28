using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parsing;

namespace CodeAnalysis.Parsing;
internal static partial class Parser
{
    private delegate T ParseNode<out T>(SyntaxTree syntaxTree, SyntaxTokenIterator iterator) where T : SyntaxNode;

    internal static CompilationUnitSyntax Parse(SyntaxTree syntaxTree) =>
        ParseCompilationUnit(syntaxTree, ParseDeclaration);

    internal static CompilationUnitSyntax ParseScript(SyntaxTree syntaxTree) =>
        ParseCompilationUnit(syntaxTree, static (syntaxTree, iterator) => ParseExpression(syntaxTree, iterator));

    private static CompilationUnitSyntax ParseCompilationUnit(SyntaxTree syntaxTree, ParseNode<SyntaxNode> parseNode)
    {
        var tokens = Scanner.Scan(syntaxTree).ToArray();
        if (tokens.Length == 0)
            return new CompilationUnitSyntax(syntaxTree, [], SyntaxFactory.EofToken(syntaxTree));

        var iterator = new SyntaxTokenIterator(tokens);

        var expressions = ParseSyntaxList(syntaxTree, iterator, [SyntaxKind.EofToken], parseNode);

        var eofToken = iterator.Match(SyntaxKind.EofToken);

        return new CompilationUnitSyntax(syntaxTree, expressions, eofToken);
    }
}
