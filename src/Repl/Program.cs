using CodeAnalysis;
using CodeAnalysis.Binding;
using CodeAnalysis.Interpretation;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

var previousCompilation = default(Compilation);
var previousEvaluatedScope = EvaluatedScope.FromGlobalBoundScope(BoundScope.GlobalScope);

while (true)
{
    var code = console.Prompt(new TextPrompt<string>(">"));

    var compilation = Compilation.CompileScript(new SourceText(code), previousCompilation);

    if (compilation.Diagnostics.HasErrorDiagnostics)
    {
        foreach (var diagnostic in compilation.Diagnostics)
            console.WriteLine(diagnostic);
        continue;
    }

    var evaluatedScope = new EvaluatedScope(previousEvaluatedScope);
    var value = Evaluator.Evaluate(compilation.BoundTrees[0], evaluatedScope);
    console.WriteLine(value);
}
