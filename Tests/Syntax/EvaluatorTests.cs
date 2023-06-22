﻿using CodeAnalysis.Symbols;

namespace CodeAnalysis.Syntax;
public sealed class EvaluatorTests
{
    [Theory]
    [MemberData(nameof(GetEvaluateResultsData))]
    public void Evaluator_Evaluates_CorrectValues(string text, object expectedValue)
    {
        var syntaxTree = SyntaxTree.Parse(text.AsMemory());
        var compilation = new Compilation(syntaxTree);
        var variables = new Dictionary<VariableSymbol, object>();
        var result = compilation.Evaluate(variables);

        Assert.Empty(result.Diagnostics);

        Assert.Equal(expectedValue, result.Value);
    }

    public static IEnumerable<object[]> GetEvaluateResultsData()
    {
        return new object[][]
        {
            new object[] { "1", 1 },
            new object[] { "+1", 1 },
            new object[] { "-1", -1 },
            new object[] { "~1", -2 },
            new object[] { "(10)", 10 },
            new object[] { "10 + 19", 29 },
            new object[] { "12 - 3", 9 },
            new object[] { "2 * 3", 6 },
            new object[] { "9 / 3", 3 },
            new object[] { "4 % 2", 0 },
            new object[] { "4 % 3", 1 },
            new object[] { "12 == 3", false },
            new object[] { "3 == 3", true },
            new object[] { "12 != 3", true },
            new object[] { "3 != 3", false },
            new object[] { "3 < 4", true },
            new object[] { "3 < 3", false },
            new object[] { "3 <= 3", true },
            new object[] { "3 <= 2", false },
            new object[] { "3 > 3", false },
            new object[] { "3 > 2", true },
            new object[] { "3 >= 3", true },
            new object[] { "3 >= 4", false },
            new object[] { "1 | 0", 1 },
            new object[] { "1 | 2", 3 },
            new object[] { "1 & 0", 0 },
            new object[] { "1 & 3", 1 },
            new object[] { "0 ^ 1", 1 },
            new object[] { "1 ^ 3", 2 },
            new object[] { "false == false", true },
            new object[] { "true == false", false },
            new object[] { "true && true", true },
            new object[] { "false || false", false },
            new object[] { "false != false", false },
            new object[] { "true != false", true },
            new object[] { "true", true },
            new object[] { "false", false },
            new object[] { "!true", false },
            new object[] { "!false", true },
            new object[] { "var a = 10;", 10 },
            new object[] { "const a = 10;", 10 },
            new object[] { "{ var a = 10; (a = 10) * a }", 100 },
            new object[] { "{ var a = 10; (a * a) }", 100 },
            new object[] { "{ var a = 0; if a == 0 a = 10 a }", 10 },
            new object[] { "{ var a = 0; if a == 4 a = 10 a }", 0 },
            new object[] { "{ var a = 0; if a == 0 a = 10 else a = 5 a }", 10 },
            new object[] { "{ var a = 0; if a == 4 a = 10 else a = 5 a }", 5 },
            new object[] { "{ var i = 10; var result = 0; while i > 0 { result = result + i; i = i - 1; } result }", 55 },
            new object[] { "{ var result = 0; for var i in 1..10 { result = result + i; } result}", 55 },
            new object[] { """
                "Hello" + " " + "World!"
                """, "Hello World!" },
        };
    }

    [Theory]
    [MemberData(nameof(GetDiagnosticsData))]
    public void Evaluator_Emits_CorrectDiagnostics(string diagnosticCase, string annotatedText, string expectedDiagnosticText)
    {
        Assert.NotNull(diagnosticCase);

        var annotated = AnnotatedText.Parse(annotatedText);
        var syntaxTree = SyntaxTree.Parse(annotated.Text);
        var compilation = new Compilation(syntaxTree);
        var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

        var expectedDiagnostics = expectedDiagnosticText.Split(Environment.NewLine);

        if (annotated.Spans.Count != expectedDiagnostics.Length)
            throw new InvalidOperationException("Invalid test: Number of marked spans does not match expected diagnostics");

        Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Count);

        for (var i = 0; i < expectedDiagnostics.Length; ++i)
        {
            var expectedMessage = expectedDiagnostics[i];
            var actualMessage = result.Diagnostics[i].Message;

            Assert.Equal(expectedMessage, actualMessage);

            var expectedSpan = annotated.Spans[i];
            var actualSpan = result.Diagnostics[i].Span;

            Assert.Equal(expectedSpan, actualSpan);
        }
    }

    public static IEnumerable<object[]> GetDiagnosticsData()
    {
        return new object[][]
        {
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.Redeclaration)}",
                """
                {
                    var x = 10;
                    var y = 100;
                    {
                        var x = 10;
                    }
                    var ⟨x⟩ = 5;
                }
                """,
                $"{DiagnosticMessage.Redeclaration("x")}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UndefinedName)}",
                "⟨x⟩ * 10",
                $"{DiagnosticMessage.UndefinedName("x")}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.ReadOnlyAssignment)}",
                """
                {
                    const x = 10;
                    x ⟨=⟩ 5;
                }
                """,
                $"{DiagnosticMessage.ReadOnlyAssignment("x")}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.InvalidConversion)}",
                """
                {
                    var x = 10;
                    ⟨x = false⟩;
                }
                """,
                $"{DiagnosticMessage.InvalidConversion(TypeSymbol.I32, TypeSymbol.Bool)}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UndefinedUnaryOperator)}",
                "⟨+⟩true",
                $"{DiagnosticMessage.UndefinedUnaryOperator("+", TypeSymbol.Bool)}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UndefinedBinaryOperator)}",
                "10 ⟨+⟩ true",
                $"{DiagnosticMessage.UndefinedBinaryOperator("+", TypeSymbol.I32, TypeSymbol.Bool)}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UnexpectedToken)} in inserted token",
                "⟨⟩",
                $"{DiagnosticMessage.UnexpectedToken(TokenKind.Identifier, TokenKind.EOF)}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UnexpectedToken)} in block statement",
                """
                {
                    ⟨)⟩⟨⟩
                """,
                $"""
                {DiagnosticMessage.UnexpectedToken(TokenKind.Identifier, TokenKind.CloseParenthesis)}
                {DiagnosticMessage.UnexpectedToken(TokenKind.CloseBrace, TokenKind.EOF)}
                """
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.InvalidConversion)} in if statement condition",
                """
                {
                    var x = 10;
                    if ⟨10⟩
                        x = 10;
                }
                """,
                $"{DiagnosticMessage.InvalidConversion(TypeSymbol.I32, TypeSymbol.Bool)}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.InvalidConversion)} in while statement condition",
                """
                {
                    var x = 10;
                    while ⟨10⟩
                        x = 10;
                }
                """,
                $"{DiagnosticMessage.InvalidConversion(TypeSymbol.I32, TypeSymbol.Bool)}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.InvalidConversion)} in for statement lower bound",
                """
                {
                    var result = 0;
                    for var i in ⟨false⟩..10
                        result = result + i;
                }
                """,
                $"{DiagnosticMessage.InvalidConversion(TypeSymbol.Bool, TypeSymbol.I32)}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.InvalidConversion)} in for statement upper bound",
                """
                {
                    var result = 0;
                    for var i in 1..⟨false⟩
                        result = result + i;
                }
                """,
                $"{DiagnosticMessage.InvalidConversion(TypeSymbol.Bool, TypeSymbol.I32)}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UndefinedName)} in for statement variable",
                """
                {
                    var result = 0;
                    for ⟨i⟩ in 1..10
                        result = result + ⟨i⟩;
                }
                """,
                $"""
                {DiagnosticMessage.UndefinedName("i")}
                {DiagnosticMessage.UndefinedName("i")}
                """
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.Redeclaration)} in for statement variable",
                """
                {
                    var result = 0;
                    var i = 0;
                    for var ⟨i⟩ in 1..10
                        result = result + i;
                }
                """,
                $"{DiagnosticMessage.Redeclaration("i")}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.ReadOnlyAssignment)} in for statement variable",
                """
                {
                    var result = 0;
                    const i = 0;
                    for ⟨i⟩ in 1..10
                        result = result + i;
                }
                """,
                $"{DiagnosticMessage.ReadOnlyAssignment("i")}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UnterminatedString)}",
                """
                ⟨"⟩text
                """,
                $"{DiagnosticMessage.UnterminatedString()}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.InvalidNumber)} for multiple '.' in a number",
                "⟨1.1.1⟩",
                $"{DiagnosticMessage.InvalidNumber("1.1.1", TypeSymbol.F32)}"
            },
        };
    }
}
