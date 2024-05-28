using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static MemberDeclarationSyntax ParseMemberDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var declaration = ParseDeclaration(syntaxTree, iterator);
        if (declaration is StructDeclarationSyntax structDeclaration)
        {
            syntaxTree.Diagnostics.ReportInvalidLocationForTypeDefinition(
                new SourceLocation(syntaxTree.SourceText, structDeclaration.IdentifierToken.Range));
        }
        if (declaration is FunctionDeclarationSyntax funcDeclaration)
        {
            syntaxTree.Diagnostics.ReportInvalidLocationForFunctionDefinition(
                new SourceLocation(syntaxTree.SourceText, funcDeclaration.IdentifierToken.Range));
        }
        return new MemberDeclarationSyntax(syntaxTree, declaration);
    }
}
