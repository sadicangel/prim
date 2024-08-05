using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static NameSyntax ParseName(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        if (iterator.Peek(1).SyntaxKind is SyntaxKind.ColonColonToken)
            return ParseQualifiedName(syntaxTree, iterator);

        return ParseSimpleName(syntaxTree, iterator);
    }
}
