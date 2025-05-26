using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    // qualified_name = simple_name ( "::" name )* ;
    private static QualifiedNameSyntax ParseQualifiedName(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        NameSyntax left = ParseSimpleName(syntaxTree, iterator);
        do
        {
            var colonColonToken = iterator.Match(SyntaxKind.ColonColonToken);
            var right = ParseSimpleName(syntaxTree, iterator);

            left = new QualifiedNameSyntax(syntaxTree, left, colonColonToken, right);
        }
        while (iterator.Current.SyntaxKind is SyntaxKind.ColonColonToken);

        return Unsafe.As<QualifiedNameSyntax>(left);
    }
}
