using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ModuleDeclarationSyntax ParseModuleDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var name = ParseSimpleNameExpression(syntaxTree, iterator);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var moduleKeyword = iterator.Match(SyntaxKind.ModuleKeyword);
        var colonOrEquals = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
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
            colonOrEquals,
            braceOpenToken,
            declarations,
            braceCloseToken);
    }
}
