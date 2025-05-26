using CodeAnalysis.Scanning;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;
internal static partial class Parser
{
    private delegate T ParseDelegate<out T>(SyntaxTree syntaxTree, SyntaxIterator iterator) where T : SyntaxNode;
    private delegate T? ParseOptionalDelegate<out T>(SyntaxTree syntaxTree, SyntaxIterator iterator) where T : SyntaxNode;

    internal static CompilationUnitSyntax Parse(SyntaxTree syntaxTree)
    {
        //ParseDelegate<SyntaxNode> parseNode = syntaxTree.ParseOptions.IsScript ? ParseExpression : ParseExpression;
        ParseDelegate<SyntaxNode> parseNode = ParseExpression;

        var tokens = Scanner.Scan(syntaxTree).ToArray();
        if (tokens.Length == 0)
        {
            return new CompilationUnitSyntax(syntaxTree, [], SyntaxToken.CreateSynthetic(SyntaxKind.EofToken, syntaxTree));
        }

        var iterator = new SyntaxIterator(tokens);

        var expressions = ParseSyntaxList(syntaxTree, iterator, [], parseNode);

        var eofToken = iterator.Match(SyntaxKind.EofToken);

        return new CompilationUnitSyntax(syntaxTree, expressions, eofToken);
    }
}
