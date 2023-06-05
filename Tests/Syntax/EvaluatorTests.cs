namespace CodeAnalysis.Syntax;
public sealed class EvaluatorTests
{
    [Theory]
    [MemberData(nameof(GetEvalueResultsData))]
    public void Evaluator_Evaluates_CorrectValues(string text, object expectedValue)
    {
        var syntaxTree = SyntaxTree.Parse(text);
        var compilation = new Compilation(syntaxTree);
        var variables = new Dictionary<Variable, object>();
        var result = compilation.Evaluate(variables);

        Assert.Empty(result.Diagnostics);

        Assert.Equal(expectedValue, result.Value);
    }

    public static IEnumerable<object[]> GetEvalueResultsData()
    {
        return new object[][]
        {
            new object[] { "1", 1L },
            new object[] { "+1", 1L },
            new object[] { "-1", -1L },
            new object[] { "10 + 19", 29L },
            new object[] { "12 - 3", 9L },
            new object[] { "2 * 3", 6L },
            new object[] { "9 / 3", 3L },
            new object[] { "(10)", 10L },
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
            new object[] { "false == false", true },
            new object[] { "true == false", false },
            new object[] { "false != false", false },
            new object[] { "true != false", true },
            new object[] { "true", true },
            new object[] { "false", false },
            new object[] { "!true", false },
            new object[] { "!false", true },
            new object[] { "{ var a = 10; (a = 10) * a }", 100L },
            new object[] { "{ var a = 0; if a == 0 a = 10 a }", 10L },
            new object[] { "{ var a = 0; if a == 4 a = 10 a }", 0L },
            new object[] { "{ var a = 0; if a == 0 a = 10 else a = 5 a }", 10L },
            new object[] { "{ var a = 0; if a == 4 a = 10 else a = 5 a }", 5L },
            new object[] { "{ var i = 10; var result = 0; while i > 0 { result = result + i; i = i - 1; } result }", 55L },
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
        var result = compilation.Evaluate(new Dictionary<Variable, object>());

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
                $"{DiagnosticMessage.InvalidConversion(typeof(long), typeof(bool))}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UndefinedUnaryOperator)}",
                "⟨+⟩true",
                $"{DiagnosticMessage.UndefinedUnaryOperator("+", typeof(bool))}"
            },
            new object[]
            {
                $"Reports {nameof(DiagnosticMessage.UndefinedBinaryOperator)}",
                "10 ⟨+⟩ true",
                $"{DiagnosticMessage.UndefinedBinaryOperator("+", typeof(long), typeof(bool))}"
            },
        };
    }
}
