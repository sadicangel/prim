using CodeAnalysis.Diagnostics;
using CodeAnalysis.Evaluation;
using CodeAnalysis.Evaluation.Values;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Tests.Evaluation;

public sealed class InterpreterTests
{
    [Fact]
    public void Interpret_NestedBinaryExpression_ReconstructsBlockScope()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var a: i32 = 40;
                var b: i32 = 2;
                var result: i32 = a + b;
            };
            """);

        var entryPoint = Assert.IsType<VariableSymbol>(compilation.EntryPoint);
        var (boundNode, diagnostics) = compilation.Bind(entryPoint);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var resultDeclaration = Assert.IsType<BoundVariableDeclaration>(block.Expressions[^1]);
        var binary = Assert.IsType<BoundBinaryExpression>(resultDeclaration.Expression);

        var result = new Interpreter().Interpret(binary);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_NestedReference_ResolvesGlobalDeclaration()
    {
        var compilation = CreateCompilation(
            """
            let answer: i32 = 42;
            let main: (str[]) -> i32 = (args) => {
                var result: i32 = answer;
            };
            """);

        var entryPoint = Assert.IsType<VariableSymbol>(compilation.EntryPoint);
        var (boundNode, diagnostics) = compilation.Bind(entryPoint);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var resultDeclaration = Assert.IsType<BoundVariableDeclaration>(block.Expressions[^1]);
        var reference = Assert.IsType<BoundReference>(resultDeclaration.Expression);

        var result = new Interpreter().Interpret(reference);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_LambdaDeclaration_CanInvokeWithArguments()
    {
        var compilation = CreateCompilation(
            """
            let addTwo: (i32) -> i32 = (x) => {
                var result: i32 = x + 2;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("addTwo", out var addTwo));

        var (boundNode, diagnostics) = compilation.Bind(addTwo);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var interpreter = new Interpreter();
        var lambda = Assert.IsType<LambdaValue>(interpreter.Interpret(boundNode));
        var i32 = Assert.IsType<StructValue>(interpreter.Interpret(compilation.Bind(compilation.GlobalModule.I32).Value));

        var result = lambda.Invoke(new InstanceValue(i32, 40));

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_StructDeclaration_EvaluatesPropertyDefaults()
    {
        var compilation = CreateCompilation(
            """
            struct Point {
                x: i32 = 1;
                y: i32 = 2;
            }
            """);

        Assert.True(compilation.GlobalModule.TryLookup<StructTypeSymbol>("Point", out var point));
        var (boundNode, diagnostics) = compilation.Bind(point);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var result = Assert.IsType<StructValue>(new Interpreter().Interpret(boundNode));
        Assert.True(point.TryLookup<PropertySymbol>("x", out var x));
        Assert.True(point.TryLookup<PropertySymbol>("y", out var y));

        Assert.Equal(1, result.Get(x).Value);
        Assert.Equal(2, result.Get(y).Value);
    }

    private static Compilation CreateCompilation(string source)
    {
        var compilation = new Compilation(new SourceText(source), new ParseOptions { IsScript = true });
        Assert.False(compilation.GetDiagnostics().HasErrorDiagnostics, string.Join(Environment.NewLine, compilation.GetDiagnostics()));
        return compilation;
    }
}
