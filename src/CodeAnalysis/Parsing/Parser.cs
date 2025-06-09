using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Scanning;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;

internal sealed record class ParseResult(CompilationUnitSyntax CompilationUnit, ImmutableArray<Diagnostic> Diagnostics);

internal static partial class Parser
{
    private delegate T ParseDelegate<out T>(SyntaxTree syntaxTree, SyntaxIterator iterator) where T : SyntaxNode;
    private delegate T? ParseOptionalDelegate<out T>(SyntaxTree syntaxTree, SyntaxIterator iterator) where T : SyntaxNode;

    internal static ParseResult Parse(SyntaxTree syntaxTree)
    {
        var diagnostics = new DiagnosticBag();

        var tokens = Scanner.Scan(syntaxTree, diagnostics).ToArray();
        if (tokens.Length == 0)
        {
            return new ParseResult(
                new CompilationUnitSyntax(syntaxTree, [], SyntaxToken.CreateSynthetic(SyntaxKind.EofToken, syntaxTree)),
                []);
        }

        var iterator = new SyntaxIterator(tokens, diagnostics);

        var declarations = ParseSyntaxList(syntaxTree, iterator, [], ParseGlobalDeclaration);

        var eofToken = iterator.Match(SyntaxKind.EofToken);

        var compilationUnit = new CompilationUnitSyntax(syntaxTree, declarations, eofToken);

        return new ParseResult(compilationUnit, [.. diagnostics]);
    }
}
