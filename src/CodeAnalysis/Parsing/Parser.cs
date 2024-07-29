using CodeAnalysis.Scanning;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;
internal static partial class Parser
{
    private delegate T ParseNode<out T>(SyntaxTree syntaxTree, SyntaxIterator iterator) where T : SyntaxNode;

    internal static CompilationUnitSyntax Parse(SyntaxTree syntaxTree)
    {
        ParseNode<SyntaxNode> parseNode = syntaxTree.IsScript ? ParseExpression : ParseDeclaration;

        var tokens = Lexer.Scan(syntaxTree).ToArray();
        if (tokens.Length == 0)
        {
            return new CompilationUnitSyntax(syntaxTree, [], SyntaxFactory.SyntheticToken(SyntaxKind.EofToken, syntaxTree));
        }

        var iterator = new SyntaxIterator(tokens);

        var expressions = ParseSyntaxList(syntaxTree, iterator, [SyntaxKind.EofToken], parseNode);

        var eofToken = iterator.Match(SyntaxKind.EofToken);

        return new CompilationUnitSyntax(syntaxTree, expressions, eofToken);
    }
}
