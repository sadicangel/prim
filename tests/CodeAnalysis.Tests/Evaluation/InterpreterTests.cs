using CodeAnalysis.Diagnostics;
using CodeAnalysis.Evaluation;
using CodeAnalysis.Evaluation.Values;
using CodeAnalysis.Semantic.ControlFlow;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.References;
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

        var result = new Interpreter(compilation).Interpret(binary);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_NestedVariableReference_ResolvesGlobalDeclaration()
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
        var reference = Assert.IsType<BoundVariableReference>(resultDeclaration.Expression);

        var result = new Interpreter(compilation).Interpret(reference);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_ArrayInitExpression_EvaluatesElements()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32[] = (args) => {
                var values: i32[] = [40, 2];
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var valuesDeclaration = Assert.IsType<BoundVariableDeclaration>(block.Expressions[^1]);
        var arrayInit = Assert.IsType<BoundArrayExpression>(valuesDeclaration.Expression);

        var result = Assert.IsType<ArrayValue>(new Interpreter(compilation).Interpret(arrayInit));

        Assert.Equal(2, result.Length);
        Assert.Equal(40, result.Elements[0].Value);
        Assert.Equal(2, result.Elements[1].Value);
    }

    [Fact]
    public void Interpret_NestedElementReference_ResolvesArrayElement()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var values: i32[] = [40, 2];
                var result: i32 = values[0] + values[1];
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
        var left = Assert.IsType<BoundElementReference>(binary.Left);

        var result = new Interpreter(compilation).Interpret(left);

        Assert.Equal(40, result.Value);
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

        var interpreter = new Interpreter(compilation);
        var lambda = Assert.IsType<LambdaValue>(interpreter.Interpret(boundNode));
        var i32 = Assert.IsType<StructValue>(interpreter.Interpret(compilation.Bind(compilation.GlobalModule.I32).Value));

        var result = lambda.Invoke(new InstanceValue(i32, 40));

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void GlobalModule_PredefinedNumericConversions_AreDeclaredOnBuiltinTypes()
    {
        var compilation = CreateCompilation(string.Empty);

        Assert.True(
            compilation.GlobalModule.I32.TryLookup<ConversionSymbol>(
                ConversionSymbol.GetConversionName(compilation.GlobalModule.I32, compilation.GlobalModule.I64),
                out var implicitConversion));
        Assert.Equal(ConversionKind.Implicit, implicitConversion.ConversionKind);

        Assert.True(
            compilation.GlobalModule.I64.TryLookup<ConversionSymbol>(
                ConversionSymbol.GetConversionName(compilation.GlobalModule.I64, compilation.GlobalModule.I32),
                out var explicitConversion));
        Assert.Equal(ConversionKind.Explicit, explicitConversion.ConversionKind);
    }

    [Fact]
    public void Interpret_ImplicitNumericConversion_EvaluatesPredefinedConversion()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i64 = (args) => {
                var result: i64 = 40;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var resultDeclaration = Assert.IsType<BoundVariableDeclaration>(block.Expressions[^1]);
        var conversionCall = Assert.IsType<BoundCallExpression>(resultDeclaration.Expression);
        var conversion = Assert.IsType<BoundConversionReference>(conversionCall.Callee);

        Assert.Equal(ConversionKind.Implicit, conversion.Conversion.ConversionKind);
        Assert.Equal("i64", conversionCall.Type.Name);

        var result = new Interpreter(compilation).Interpret(conversionCall);

        Assert.Equal(40L, result.Value);
        Assert.Equal("i64", result.Type.Name);
    }

    [Fact]
    public void Interpret_ExplicitNumericConversion_EvaluatesPredefinedConversion()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var result: i32 = 42i64 as i32;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var resultDeclaration = Assert.IsType<BoundVariableDeclaration>(block.Expressions[^1]);
        var conversionCall = Assert.IsType<BoundCallExpression>(resultDeclaration.Expression);
        var conversion = Assert.IsType<BoundConversionReference>(conversionCall.Callee);

        Assert.Equal(ConversionKind.Explicit, conversion.Conversion.ConversionKind);
        Assert.Equal("i32", conversionCall.Type.Name);

        var result = new Interpreter(compilation).Interpret(conversionCall);

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

        var result = Assert.IsType<StructValue>(new Interpreter(compilation).Interpret(boundNode));
        Assert.True(point.TryLookup<PropertySymbol>("x", out var x));
        Assert.True(point.TryLookup<PropertySymbol>("y", out var y));

        Assert.Equal(1, result.Get(x).Value);
        Assert.Equal(2, result.Get(y).Value);
    }

    [Fact]
    public void Interpret_StructExpression_OverridesSpecifiedPropertiesAndKeepsDefaults()
    {
        var compilation = CreateCompilation(
            """
            struct Point {
                x: i32 = 1;
                y: i32 = 2;
            }
            let main: (str[]) -> Point = (args) => {
                var point: Point = Point { x = 40 };
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<StructTypeSymbol>("Point", out var point));
        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var pointDeclaration = Assert.IsType<BoundVariableDeclaration>(block.Expressions[^1]);
        var structExpression = Assert.IsType<BoundStructExpression>(pointDeclaration.Expression);

        var result = Assert.IsType<InstanceValue>(new Interpreter(compilation).Interpret(structExpression));
        Assert.True(point.TryLookup<PropertySymbol>("x", out var x));
        Assert.True(point.TryLookup<PropertySymbol>("y", out var y));

        Assert.Equal(40, result.Get(x).Value);
        Assert.Equal(2, result.Get(y).Value);
    }

    [Fact]
    public void Interpret_BoundInvocationExpression_InvokesLambda()
    {
        var compilation = CreateCompilation(
            """
            let addTwo: (i32) -> i32 = (x) => {
                var result: i32 = x + 2;
            };
            let main: (str[]) -> i32 = (args) => {
                var result: i32 = addTwo(40);
            };
            """);

        var entryPoint = Assert.IsType<VariableSymbol>(compilation.EntryPoint);
        var (boundNode, diagnostics) = compilation.Bind(entryPoint);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var resultDeclaration = Assert.IsType<BoundVariableDeclaration>(block.Expressions[^1]);
        var invocation = Assert.IsType<BoundCallExpression>(resultDeclaration.Expression);

        var result = new Interpreter(compilation).Interpret(invocation);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_PropertyAccessReference_ResolvesInstancePropertyValues()
    {
        var compilation = CreateCompilation(
            """
            struct Point {
                x: i32 = 0;
                y: i32 = 0;
            }
            let main: (str[]) -> i32 = (args) => {
                var a = 40;
                var b = 0;
                b = -1 * 2;
                let p = Point { x = a, y = b };

                var d: i32[] = [1, 2, 3];
                let r = p.x + p.y;
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
        Assert.IsType<BoundPropertyReference>(binary.Left);
        Assert.IsType<BoundPropertyReference>(binary.Right);

        var result = new Interpreter(compilation).Interpret(block);

        Assert.Equal(38, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_BlockContainingOnlyNop_ReturnsUnit()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> unit = (args) => {
                ;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        Assert.IsType<BoundNopExpression>(block.Expressions[0]);

        var result = new Interpreter(compilation).Interpret(block);

        Assert.Same(Unit.Value, result.Value);
        Assert.Equal("unit", result.Type.Name);
    }

    [Fact]
    public void Interpret_NestedAssignmentExpression_UpdatesLocalValue()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var result: i32 = 40;
                result = result + 2;
            };
            """);

        var entryPoint = Assert.IsType<VariableSymbol>(compilation.EntryPoint);
        var (boundNode, diagnostics) = compilation.Bind(entryPoint);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var assignment = Assert.IsType<BoundAssignmentExpression>(block.Expressions[1]);

        var result = new Interpreter(compilation).Interpret(assignment);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_BlockWithCapturedAssignment_UpdatesCapturedValue()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var value: i32 = 40;
                var addTwo: () -> i32 = () => {
                    value = value + 2;
                };
                addTwo();
            };
            """);

        var entryPoint = Assert.IsType<VariableSymbol>(compilation.EntryPoint);
        var (boundNode, diagnostics) = compilation.Bind(entryPoint);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);

        var result = new Interpreter(compilation).Interpret(block);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_BlockWithElementAssignment_UpdatesArrayValue()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var values: i32[] = [40, 0];
                values[1] = values[0] + 2;
                values[1];
            };
            """);

        var entryPoint = Assert.IsType<VariableSymbol>(compilation.EntryPoint);
        var (boundNode, diagnostics) = compilation.Bind(entryPoint);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);

        var result = new Interpreter(compilation).Interpret(block);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_IfElseExpression_EvaluatesElseBranchWhenConditionIsFalse()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var result: i32 = if (2 < 0) 40 else 2;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var resultDeclaration = Assert.IsType<BoundVariableDeclaration>(block.Expressions[^1]);
        var ifElse = Assert.IsType<BoundIfElseExpression>(resultDeclaration.Expression);

        var result = new Interpreter(compilation).Interpret(ifElse);

        Assert.Equal(2, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_IfExpressionWithoutElse_ReturnsThenValueWrappedInUnionWhenConditionIsTrue()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 | unit = (args) => {
                if (2 > 0) 40;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var ifElse = Assert.IsType<BoundIfElseExpression>(block.Expressions[0]);

        var result = Assert.IsType<UnionValue>(new Interpreter(compilation).Interpret(ifElse));
        var member = Assert.IsType<InstanceValue>(result.Value);

        Assert.Equal("i32 | unit", result.Type.Name);
        Assert.Equal(40, member.Value);
        Assert.Equal("i32", member.Type.Name);
    }

    [Fact]
    public void Interpret_IfExpressionWithoutElse_ReturnsUnitWrappedInUnionWhenConditionIsFalse()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 | unit = (args) => {
                if (2 < 0) 40;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var ifElse = Assert.IsType<BoundIfElseExpression>(block.Expressions[0]);

        var result = Assert.IsType<UnionValue>(new Interpreter(compilation).Interpret(ifElse));
        var member = Assert.IsType<InstanceValue>(result.Value);

        Assert.Equal("i32 | unit", result.Type.Name);
        Assert.Same(Unit.Value, member.Value);
        Assert.Equal("unit", member.Type.Name);
    }

    [Fact]
    public void Interpret_WhileExpression_EvaluatesBodyUntilConditionIsFalse()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var i: i32 = 0;
                while (i < 3) {
                    i = i + 1;
                }
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var whileExpression = Assert.IsType<BoundWhileExpression>(block.Expressions[1]);

        var result = new Interpreter(compilation).Interpret(whileExpression);

        Assert.Equal(3, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_WhileExpression_ReturnsDefaultValueWhenConditionStartsFalse()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                var i: i32 = 5;
                while (i < 3) {
                    i = i + 1;
                }
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var whileExpression = Assert.IsType<BoundWhileExpression>(block.Expressions[1]);

        var result = new Interpreter(compilation).Interpret(whileExpression);

        Assert.Equal(0, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_WhileExpression_ReturnsBreakValue()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> i32 = (args) => {
                while (true) {
                    break 42;
                }
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var whileExpression = Assert.IsType<BoundWhileExpression>(block.Expressions[0]);

        var result = new Interpreter(compilation).Interpret(whileExpression);

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    [Fact]
    public void Interpret_WhileExpression_ContinueSkipsToNextIteration()
    {
        var compilation = CreateCompilation(
            """
            let main: (str[]) -> unit = (args) => {
                var i: i32 = 0;
                while (i < 3) {
                    i = i + 1;
                    continue;
                }
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("main", out var mainSymbol));
        var (boundNode, diagnostics) = compilation.Bind(mainSymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var main = Assert.IsType<BoundVariableDeclaration>(boundNode);
        var lambda = Assert.IsType<BoundLambdaExpression>(main.Expression);
        var block = Assert.IsType<BoundBlockExpression>(lambda.Body);
        var whileExpression = Assert.IsType<BoundWhileExpression>(block.Expressions[1]);

        var result = new Interpreter(compilation).Interpret(whileExpression);

        Assert.Same(Unit.Value, result.Value);
        Assert.Equal("unit", result.Type.Name);
    }

    [Fact]
    public void Interpret_LambdaDeclaration_ReturnExpression_ExitsEarly()
    {
        var compilation = CreateCompilation(
            """
            let early: () -> i32 = () => {
                return 42;
                0;
            };
            """);

        Assert.True(compilation.GlobalModule.TryLookup<VariableSymbol>("early", out var earlySymbol));
        var (boundNode, diagnostics) = compilation.Bind(earlySymbol);
        Assert.False(diagnostics.HasErrorDiagnostics, string.Join(Environment.NewLine, diagnostics));

        var interpreter = new Interpreter(compilation);
        var lambda = Assert.IsType<LambdaValue>(interpreter.Interpret(boundNode));

        var result = lambda.Invoke();

        Assert.Equal(42, result.Value);
        Assert.Equal("i32", result.Type.Name);
    }

    private static Compilation CreateCompilation(string source)
    {
        var compilation = new Compilation(new SourceText(source), new ParseOptions { IsScript = true });
        Assert.False(compilation.GetDiagnostics().HasErrorDiagnostics, string.Join(Environment.NewLine, compilation.GetDiagnostics()));
        return compilation;
    }
}
