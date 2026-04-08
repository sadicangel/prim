using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Tests.Binding;

public sealed class ControlFlowBindingTests
{
    [Fact]
    public void Bind_BreakExpressionOutsideLoop_ReportsDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> unit = (args) => {
                break;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (_, diagnostics) = compilation.Bind(mainSymbol);

        var diagnostic = Assert.Single(diagnostics.Where(d => d.Message == "No enclosing loop out of which to break or continue"));

        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
    }

    [Fact]
    public void Bind_ReturnExpressionOutsideLambda_ReportsDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            let value: i32 = return 42;
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("value", out var valueSymbol));
        var (_, diagnostics) = compilation.Bind(valueSymbol);

        var diagnostic = Assert.Single(diagnostics.Where(d => d.Message == "No enclosing function out of which to return"));

        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
    }

    private static Compilation CreateCompilation(string source)
    {
        var compilation = new Compilation(new SourceText(source), new ParseOptions { IsScript = true });
        Assert.False(compilation.GetDiagnostics().HasErrorDiagnostics, string.Join(Environment.NewLine, compilation.GetDiagnostics()));
        return compilation;
    }
}
