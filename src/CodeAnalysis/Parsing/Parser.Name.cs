using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // name = simple_name | qualified_name
    private static NameSyntax ParseName(SyntaxIterator iterator)
    {
        if (iterator.Peek(1).SyntaxKind is SyntaxKind.ColonColonToken)
            return ParseQualifiedName(iterator);

        return ParseSimpleName(iterator);
    }
}
