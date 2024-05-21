using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static LocalDeclarationSyntax ParseLocalDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var declaration = ParseDeclaration(syntaxTree, iterator);
        if (declaration is StructDeclarationSyntax structDeclaration)
        {
            syntaxTree.Diagnostics.ReportInvalidLocationForTypeDefinition(
                new SourceLocation(
                    syntaxTree.SourceText,
                    new Range(structDeclaration.IdentifierToken.Range.Start, structDeclaration.TypeToken.Range.End)));
        }
        return new LocalDeclarationSyntax(syntaxTree, declaration);
    }
}
