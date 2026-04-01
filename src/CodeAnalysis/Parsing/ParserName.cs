using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal static class ParserName
{
    extension(SyntaxTokenStream stream)
    {
        // name = simple_name | qualified_name
        public NameSyntax ParseName()
        {
            if (stream.Peek(1).SyntaxKind is SyntaxKind.ColonColonToken)
                return stream.ParseQualifiedName();

            return stream.ParseSimpleName();
        }

        // simple_name = 'identifier'
        public SimpleNameSyntax ParseSimpleName()
        {
            var identifierToken = stream.Match(SyntaxKind.IdentifierToken);

            return new SimpleNameSyntax(identifierToken);
        }

        // qualified_name = simple_name ( "::" name )* ;
        public QualifiedNameSyntax ParseQualifiedName()
        {
            NameSyntax left = stream.ParseSimpleName();
            do
            {
                var colonColonToken = stream.Match(SyntaxKind.ColonColonToken);
                var right = stream.ParseSimpleName();

                left = new QualifiedNameSyntax(left, colonColonToken, right);
            } while (stream.Current.SyntaxKind is SyntaxKind.ColonColonToken);

            return Unsafe.As<QualifiedNameSyntax>(left);
        }
    }
}
