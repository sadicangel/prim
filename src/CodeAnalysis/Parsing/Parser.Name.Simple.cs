using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // simple_name = 'identifier'
    private static SimpleNameSyntax ParseSimpleName(SyntaxTokenStream stream)
    {
        var identifierToken = stream.Match(SyntaxKind.IdentifierToken);

        return new SimpleNameSyntax(identifierToken);
    }
}
