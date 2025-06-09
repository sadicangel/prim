using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    // simple_name = 'identifier'
    private static SimpleNameSyntax ParseSimpleName(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);

        return new SimpleNameSyntax(syntaxTree, identifierToken);
    }
}
