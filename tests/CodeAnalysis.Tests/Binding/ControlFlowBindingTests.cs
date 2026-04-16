using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Declarations;
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
    public void Bind_ReturnStatementAsDirectLambdaBody_ProducesNoDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            let value: () -> i32 = () => return 42;
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("value", out var valueSymbol));
        var (_, diagnostics) = compilation.Bind(valueSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));
    }

    [Fact]
    public void Bind_GlobalModule_BindsMembersThroughCompilation()
    {
        var compilation = CreateCompilation(
            """
            struct Point {
                x: i32 = 1;
            }
            let main: () -> i32 = () => 42;
            """);

        var (boundNode, diagnostics) = compilation.Bind(compilation.GlobalModule);

        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var module = Assert.IsType<BoundModuleDeclaration>(boundNode);
        Assert.Contains(module.Members, member => member is BoundStructDeclaration { StructSymbol.Name: "Point" });
        Assert.Contains(module.Members, member => member is BoundVariableDeclaration { VariableSymbol.Name: "main" });
    }

    private static Compilation CreateCompilation(string source)
    {
        var compilation = new Compilation(new SourceText(source), new ParseOptions { IsScript = true });
        Assert.False(compilation.GetDiagnostics().HasErrorDiagnostics, string.Join(Environment.NewLine, compilation.GetDiagnostics()));
        return compilation;
    }
}
