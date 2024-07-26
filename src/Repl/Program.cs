using CodeAnalysis;
using CodeAnalysis.Lowering;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

var previousCompilation = default(Compilation);
var previousEvaluation = default(Evaluation);

while (true)
{
    var code = console.Prompt(new TextPrompt<string>(">").DefaultValue("""
        i:= 10;
        while (i >= 0) {
            i = i - 1;
        }
        """));

    var compilation = Compilation.CompileScript(new SourceText(code), previousCompilation);

    if (compilation.Diagnostics.Count > 0)
    {
        foreach (var diagnostic in compilation.Diagnostics)
            console.WriteLine(diagnostic);
        if (compilation.Diagnostics.HasErrorDiagnostics)
            continue;
    }

    foreach (var boundTree in compilation.BoundTrees)
    {
        console.WriteLine(boundTree);
        console.WriteLine(Lowerer.Lower(boundTree));
    }

    var evaluation = Evaluation.Evaluate(compilation, previousEvaluation);

    console.WriteLine(evaluation.Values[0]);

    previousCompilation = compilation;
    previousEvaluation = evaluation;
}
