using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // qualified_name = simple_name ( "::" name )* ;
    private static QualifiedNameSyntax ParseQualifiedName(SyntaxTokenStream stream)
    {
        NameSyntax left = ParseSimpleName(stream);
        do
        {
            var colonColonToken = stream.Match(SyntaxKind.ColonColonToken);
            var right = ParseSimpleName(stream);

            left = new QualifiedNameSyntax(left, colonColonToken, right);
        } while (stream.Current.SyntaxKind is SyntaxKind.ColonColonToken);

        return Unsafe.As<QualifiedNameSyntax>(left);
    }
}
