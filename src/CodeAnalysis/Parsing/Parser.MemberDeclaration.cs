using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static MemberDeclarationSyntax ParseMemberDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var declaration = ParseDeclaration(syntaxTree, iterator);
        if (declaration is TypeDeclarationSyntax structDeclaration)
        {
            syntaxTree.Diagnostics.ReportInvalidLocationForTypeDefinition(
                new SourceLocation(
                    syntaxTree.SourceText,
                    new Range(structDeclaration.IdentifierToken.Range.Start, structDeclaration.TypeToken.Range.End)));
        }
        if (declaration is FunctionDeclarationSyntax funcDeclaration)
        {
            syntaxTree.Diagnostics.ReportInvalidLocationForFunctionDefinition(
                new SourceLocation(
                    syntaxTree.SourceText,
                    new Range(funcDeclaration.IdentifierToken.Range.Start, funcDeclaration.Type.Range.End)));
        }
        return new MemberDeclarationSyntax(syntaxTree, declaration);
    }
}
