using CodeAnalysis;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

var previousCompilation = default(Compilation);
var previousEvaluation = default(Evaluation);

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

    var evaluation = Evaluation.Evaluate(compilation, previousEvaluation);

    console.WriteLine(evaluation.Values[0]);

    previousCompilation = compilation;
    previousEvaluation = evaluation;
}
