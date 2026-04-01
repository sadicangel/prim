using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // name = simple_name | qualified_name
    private static NameSyntax ParseName(SyntaxTokenStream stream)
    {
        if (stream.Peek(1).SyntaxKind is SyntaxKind.ColonColonToken)
            return ParseQualifiedName(stream);

        return ParseSimpleName(stream);
    }
}
