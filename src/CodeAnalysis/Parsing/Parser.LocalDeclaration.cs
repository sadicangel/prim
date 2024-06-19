using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static LocalDeclarationSyntax ParseLocalDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var declaration = ParseDeclaration(syntaxTree, iterator);
        if (!syntaxTree.IsScript && declaration is StructDeclarationSyntax structDeclaration)
        {
            syntaxTree.Diagnostics.ReportInvalidLocationForTypeDefinition(
                new SourceLocation(syntaxTree.SourceText, structDeclaration.IdentifierToken.Range));
        }
        return new LocalDeclarationSyntax(syntaxTree, declaration);
    }
}
