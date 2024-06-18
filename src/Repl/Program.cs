using CodeAnalysis.Binding;
using CodeAnalysis.Evaluation;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

const string Code = """
name: str = "" + 2;
name
""";

var syntaxTree = SyntaxTree.ParseScript(new SourceText(Code));
var boundScope = new BoundScope();
var boundTree = BoundTree.Bind(syntaxTree, boundScope);
var evaluatedScope = new EvaluatedScope(EvaluatedScope.FromGlobalBoundScope(boundScope.GlobalScope));
var (value, diagnostics) = Evaluator.Evaluate(boundTree, evaluatedScope);

if (diagnostics.Count > 0)
{
    foreach (var diagnostic in diagnostics)
        console.WriteLine(diagnostic);
}

console.WriteLine(value);
