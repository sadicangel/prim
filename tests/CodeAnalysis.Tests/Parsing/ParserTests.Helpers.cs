using CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    private static ExpressionSyntax ParseExpression(string text)
    {
        var sourceText = new SourceText($"__test:={text};");
        var syntaxTree = new SyntaxTree(sourceText);
        var diagnostics = syntaxTree.Diagnostics.ToArray();

        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var declaration = Assert.Single(syntaxTree.CompilationUnit.Declarations);
        return declaration.Initializer;
    }

    private static ExpressionSyntax ParseStatement(string text) => ParseExpression(text);
    private static GlobalDeclarationSyntax ParseGlobalDeclaration(string text)
    {
        var sourceText = new SourceText(text);
        var syntaxTree = new SyntaxTree(sourceText);
        var diagnostics = syntaxTree.Diagnostics.ToArray();

        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        return Assert.Single(syntaxTree.CompilationUnit.Declarations);
    }

    private static ExpressionSyntax ParseExpressionWithDiagnostics(string text, out Diagnostic[] diagnostics)
    {
        var sourceText = new SourceText($"__test:={text};");
        var syntaxTree = new SyntaxTree(sourceText);
        diagnostics = syntaxTree.Diagnostics.ToArray();

        var declaration = Assert.Single(syntaxTree.CompilationUnit.Declarations);
        return declaration.Initializer;
    }
}
