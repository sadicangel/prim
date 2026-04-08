using CodeAnalysis.Scanning;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;
using CodeAnalysis.Text;

namespace CodeAnalysis.Parsing;

internal delegate T ParseDelegate<out T>(SyntaxTokenStream stream) where T : SyntaxNode;

internal delegate T? ParseOptionalDelegate<out T>(SyntaxTokenStream stream) where T : SyntaxNode;

internal static class Parser
{
    extension(SyntaxTokenStream)
    {
        public static Result<CompilationUnitSyntax> Parse(SourceText sourceText, ParseOptions options)
        {
            var (syntaxTokens, scanDiagnostics) = Scanner.Scan(sourceText);
            if (syntaxTokens.Length == 0)
            {
                return new Result<CompilationUnitSyntax>(
                    new CompilationUnitSyntax(null, [], SyntaxToken.CreateSynthetic(SyntaxKind.EofToken)),
                    scanDiagnostics);
            }

            var stream = new SyntaxTokenStream(sourceText, syntaxTokens);

            // TODO: Maybe factor out module declaration?
            var all = stream.ParseSyntaxList([], ParserDeclaration.ParseGlobalDeclaration);
            var module = all.SingleOrDefault(x => x is ModuleDeclarationSyntax) as ModuleDeclarationSyntax;
            var declarations = new SyntaxList<DeclarationSyntax>([.. all.Where(x => x is not ModuleDeclarationSyntax)]);
            var eofToken = stream.Match(SyntaxKind.EofToken);


            return new Result<CompilationUnitSyntax>(
                new CompilationUnitSyntax(module, declarations, eofToken),
                [.. scanDiagnostics, .. stream.Diagnostics]);
        }
    }
}
