using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions.Declarations;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ModuleDeclarationSyntax ParseModuleDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var name = ParseSimpleName(syntaxTree, iterator);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var moduleKeyword = iterator.Match(SyntaxKind.ModuleKeyword);
        var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var declarations = ParseSyntaxList(
            syntaxTree,
            iterator,
            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            ParseDeclaration);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);

        return new ModuleDeclarationSyntax(
            syntaxTree,
            name,
            colonToken,
            moduleKeyword,
            equalsToken,
            braceOpenToken,
            declarations,
            braceCloseToken);
    }
}
