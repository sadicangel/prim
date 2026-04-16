using CodeAnalysis.Diagnostics;
using CodeAnalysis.Parsing;
using CodeAnalysis.Scanning;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    private static ExpressionSyntax ParseExpression(string text)
    {
        var sourceText = new SourceText(text);
        var (tokens, scanDiagnostics) = Scanner.Scan(sourceText);

        Assert.False(scanDiagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, scanDiagnostics));

        var stream = new SyntaxTokenStream(sourceText, tokens);
        var expression = stream.ParseExpression();
        _ = stream.Match(SyntaxKind.EofToken);

        Assert.False(stream.Diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, stream.Diagnostics));

        return expression;
    }

    private static StatementSyntax ParseStatement(string text)
    {
        var sourceText = new SourceText(text);
        var (tokens, scanDiagnostics) = Scanner.Scan(sourceText);

        Assert.False(scanDiagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, scanDiagnostics));

        var stream = new SyntaxTokenStream(sourceText, tokens);
        var statement = stream.ParseStatement();
        _ = stream.Match(SyntaxKind.EofToken);

        Assert.False(stream.Diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, stream.Diagnostics));

        return statement;
    }
}
