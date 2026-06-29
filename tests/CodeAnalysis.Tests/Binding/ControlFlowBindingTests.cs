using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic;

namespace CodeAnalysis.Tests.Binding;

public sealed class BinderTests
{
    [Fact]
    public void Bind_ModuleHeader_DeclaresPathAndPlacesMembersInCurrentModule()
    {
        var (compilation, _, diagnostics) = Bind(
            """
            a.b :: module;
            Point := type { };
            value := 1;
            """);

        AssertNoErrors(diagnostics);
        var a = Assert.IsType<ModuleSymbol>(compilation.GlobalModule.Lookup("a"));
        var b = Assert.IsType<ModuleSymbol>(a.Lookup("b"));
        Assert.NotNull(b.Lookup<StructTypeSymbol>("Point"));
        var value = Assert.IsType<VariableSymbol>(b.Lookup("value"));
        Assert.Equal(PredefinedTypeNames.I32, value.Type.Name.ToString());
        Assert.Null(compilation.GlobalModule.Lookup<StructTypeSymbol>("Point"));
    }

    [Fact]
    public void Bind_ModuleDeclaration_NotFirstOrRepeated_ReportsDiagnostic()
    {
        var (_, _, diagnostics) = Bind(
            """
            value := 1;
            a :: module;
            b :: module;
            """);

        Assert.Equal(2, diagnostics.Count(d => d.Id == DiagnosticId.InvalidModuleDeclaration));
    }

    [Fact]
    public void Bind_QualifiedTypeAndVariableDeclarations_ReportDiagnostic()
    {
        var (compilation, _, diagnostics) = Bind(
            """
            a.b :: module;
            a.Point := type { };
            a.value := 1;
            """);

        Assert.Equal(2, diagnostics.Count(d => d.Id == DiagnosticId.InvalidQualifiedDeclarationName));
        var a = Assert.IsType<ModuleSymbol>(compilation.GlobalModule.Lookup("a"));
        var b = Assert.IsType<ModuleSymbol>(a.Lookup("b"));
        Assert.Null(b.Lookup<StructTypeSymbol>("Point"));
        Assert.Null(b.Lookup<VariableSymbol>("value"));
    }

    [Fact]
    public void Bind_QualifiedReferences_ResolveThroughSharedGlobalModule()
    {
        var (mathCompilation, _, mathDiagnostics) = Bind(
            """
            math :: module;
            Point := type { };
            one := 1;
            """);
        AssertNoErrors(mathDiagnostics);

        var (appCompilation, _, appDiagnostics) = Bind(
            """
            app :: module;
            p: math.Point = math.Point { };
            x := math.one + 2;
            """,
            mathCompilation.GlobalModule);

        AssertNoErrors(appDiagnostics);
        var app = Assert.IsType<ModuleSymbol>(appCompilation.GlobalModule.Lookup("app"));
        var p = Assert.IsType<VariableSymbol>(app.Lookup("p"));
        var x = Assert.IsType<VariableSymbol>(app.Lookup("x"));
        Assert.Equal("Point", p.Type.Name.ToString());
        Assert.Equal(PredefinedTypeNames.I32, x.Type.Name.ToString());
    }

    [Fact]
    public void Bind_QualifiedTypeReference_AmbiguousBetweenCurrentAndGlobal_ReportsDiagnostic()
    {
        var (mathCompilation, _, mathDiagnostics) = Bind(
            """
            math :: module;
            Point := type {
                x: i32 = 0;
            };
            """);
        AssertNoErrors(mathDiagnostics);

        var (appMathCompilation, _, appMathDiagnostics) = Bind(
            """
            app.math :: module;
            Point := type {
                y: bool = false;
            };
            """,
            mathCompilation.GlobalModule);
        AssertNoErrors(appMathDiagnostics);

        var (_, _, appDiagnostics) = Bind(
            """
            app :: module;
            p: math.Point = math.Point { x = 1 };
            """,
            appMathCompilation.GlobalModule);

        Assert.Contains(appDiagnostics, d => d.Id == DiagnosticId.AmbiguousSymbol);
    }

    [Fact]
    public void Bind_QualifiedExpression_AmbiguousBetweenCurrentAndGlobal_ReportsDiagnostic()
    {
        var (mathCompilation, _, mathDiagnostics) = Bind(
            """
            math :: module;
            one := 1;
            """);
        AssertNoErrors(mathDiagnostics);

        var (appMathCompilation, _, appMathDiagnostics) = Bind(
            """
            app.math :: module;
            one := false;
            """,
            mathCompilation.GlobalModule);
        AssertNoErrors(appMathDiagnostics);

        var (_, _, appDiagnostics) = Bind(
            """
            app :: module;
            x := math.one;
            """,
            appMathCompilation.GlobalModule);

        Assert.Contains(appDiagnostics, d => d.Id == DiagnosticId.AmbiguousSymbol);
    }

    [Fact]
    public void Bind_DottedExpression_FirstSegmentLocal_ShadowsGlobalModule()
    {
        var (compilation, declarations, diagnostics) = Bind(
            """
            math :: module;
            one := 1;
            Point := type {
                one: bool = false;
            };
            read: (Point) -> bool = (math) => math.one;
            """);

        AssertNoErrors(diagnostics);
        var read = Assert.Single(declarations, d => d.Symbol.Name == "read");
        var lambda = Assert.IsType<LambdaExpressionNode>(read.Initializer);
        var member = Assert.IsType<MemberReferenceNode>(lambda.Body);
        Assert.Equal(PredefinedTypeNames.Bool, member.Type.Name.ToString());
        Assert.IsType<VariableReferenceNode>(member.Receiver);
        Assert.IsType<ModuleSymbol>(compilation.GlobalModule.Lookup("math"));
    }

    [Fact]
    public void Bind_UnqualifiedObjectInitializer_UsesCurrentModule()
    {
        var (compilation, _, diagnostics) = Bind(
            """
            app :: module;
            Point := type {
                x: i32 = 0;
            };
            p: Point = Point { x = 1 };
            """);

        AssertNoErrors(diagnostics);
        var app = Assert.IsType<ModuleSymbol>(compilation.GlobalModule.Lookup("app"));
        var point = Assert.IsType<StructTypeSymbol>(app.Lookup("Point"));
        var p = Assert.IsType<VariableSymbol>(app.Lookup("p"));
        Assert.Same(point, p.Type);
    }
    [Fact]
    public void Bind_UnaryAndBinaryOperators_ResolvePredefinedOperators()
    {
        var (compilation, _, diagnostics) = Bind(
            """
            value := -1 + 2;
            flag := !false;
            """);

        AssertNoErrors(diagnostics);
        var value = Assert.IsType<VariableSymbol>(compilation.GlobalModule.Lookup("value"));
        var flag = Assert.IsType<VariableSymbol>(compilation.GlobalModule.Lookup("flag"));
        Assert.Equal(PredefinedTypeNames.I32, value.Type.Name.ToString());
        Assert.Equal(PredefinedTypeNames.Bool, flag.Type.Name.ToString());
    }

    [Fact]
    public void Bind_ReturnAndObjectInitializer_WorkInsideLambda_WhileBreakOutsideLoopReportsDiagnostic()
    {
        var (compilation, declarations, diagnostics) = Bind(
            """
            Point := type {
                x: i32 = 0;
            };
            make: () -> Point = () => return Point { x = 1 };
            badBreak: () -> unit = () => {
                break;
            };
            """);

        Assert.Contains(diagnostics, d => d.Id == DiagnosticId.InvalidBreakOrContinue);
        Assert.DoesNotContain(diagnostics, d => d.Id == DiagnosticId.InvalidReturn);
        var point = Assert.IsType<StructTypeSymbol>(compilation.GlobalModule.Lookup("Point"));
        Assert.NotNull(point.Lookup<PropertySymbol>("x"));

        var make = Assert.Single(declarations, d => d.Symbol.Name == "make");
        var lambda = Assert.IsType<LambdaExpressionNode>(make.Initializer);
        Assert.IsType<ReturnExpressionNode>(lambda.Body);
    }

    private static (Compilation Compilation, ImmutableArray<DeclarationNode> Declarations, ImmutableArray<Diagnostic> Diagnostics) Bind(
        string source,
        ModuleSymbol? globalModule = null)
    {
        var compilation = globalModule is null
            ? new Compilation(new SourceText(source))
            : new Compilation(new SourceText(source), globalModule);
        var syntaxDiagnostics = compilation.GetDiagnostics().ToImmutableArray();
        AssertNoErrors(syntaxDiagnostics);
        var (declarations, diagnostics) = compilation.Bind();
        return (compilation, declarations, diagnostics);
    }

    private static void AssertNoErrors(IEnumerable<Diagnostic> diagnostics) =>
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));
}
