using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // qualified_name = simple_name ( "::" name )* ;
    private static QualifiedNameSyntax ParseQualifiedName(SyntaxIterator iterator)
    {
        NameSyntax left = ParseSimpleName(iterator);
        do
        {
            var colonColonToken = iterator.Match(SyntaxKind.ColonColonToken);
            var right = ParseSimpleName(iterator);

            left = new QualifiedNameSyntax(left, colonColonToken, right);
        } while (iterator.Current.SyntaxKind is SyntaxKind.ColonColonToken);

        return Unsafe.As<QualifiedNameSyntax>(left);
    }
}
