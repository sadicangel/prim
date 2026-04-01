using CodeAnalysis.Scanning;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;
using CodeAnalysis.Text;

namespace CodeAnalysis.Parsing;

internal static partial class Parser
{
    private delegate T ParseDelegate<out T>(SyntaxIterator iterator) where T : SyntaxNode;

    private delegate T? ParseOptionalDelegate<out T>(SyntaxIterator iterator) where T : SyntaxNode;

    internal static Result<CompilationUnitSyntax> Parse(SourceText sourceText)
    {
        var (syntaxTokens, scanDiagnostics) = Scanner.Scan(sourceText);
        if (syntaxTokens.Length == 0)
        {
            return new Result<CompilationUnitSyntax>(
                new CompilationUnitSyntax(null, [], SyntaxToken.CreateSynthetic(SyntaxKind.EofToken)),
                scanDiagnostics);
        }

        var iterator = new SyntaxIterator(sourceText, syntaxTokens);

        // TODO: Maybe factor out module declaration?
        var all = ParseSyntaxList(iterator, [], ParseGlobalDeclaration);
        var module = all.SingleOrDefault(x => x is ModuleDeclarationSyntax) as ModuleDeclarationSyntax;
        var declarations = new SyntaxList<DeclarationSyntax>([.. all.Where(x => x is not ModuleDeclarationSyntax)]);
        var eofToken = iterator.Match(SyntaxKind.EofToken);


        return new Result<CompilationUnitSyntax>(
            new CompilationUnitSyntax(module, declarations, eofToken),
            [.. scanDiagnostics, .. iterator.Diagnostics]);
    }
}
