using CodeAnalysis.Binding;
using CodeAnalysis.Evaluation;
using CodeAnalysis.Evaluation.Values;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

const string Code = """
Point2d: struct = {
    x: f32 = 0f;
    y: f32 = 0f;
}

p: Point2d = Point2d {
    .x = 2f,
    .y = 6f,
}

p
""";

var (value, diagnostics) = Evaluate(new SourceText(Code));

if (diagnostics.Count > 0)
{
    foreach (var diagnostic in diagnostics)
        console.WriteLine(diagnostic);
}

console.WriteLine(value);

static EvaluatedResult Evaluate(SourceText sourceText)
{
    var syntaxTree = SyntaxTree.ParseScript(sourceText);
    if (syntaxTree.Diagnostics.HasErrorDiagnostics)
    {
        return new EvaluatedResult(LiteralValue.Unit, syntaxTree.Diagnostics);
    }
    var boundScope = new BoundScope();
    var boundTree = BoundTree.Bind(syntaxTree, boundScope);
    if (boundTree.Diagnostics.HasErrorDiagnostics)
    {
        return new EvaluatedResult(LiteralValue.Unit, boundTree.Diagnostics);
    }
    var evaluatedScope = new EvaluatedScope(EvaluatedScope.FromGlobalBoundScope(boundScope.GlobalScope));
    return Evaluator.Evaluate(boundTree, evaluatedScope);
}
