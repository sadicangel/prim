using CodeAnalysis;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

var previousCompilation = default(Compilation);
var previousEvaluation = default(Evaluation);

while (true)
{
    var @default = Markup.Escape("""
        a: module = {
            b: module = {
                c: i32 = 0;
            }
        }
        z: () -> i32 = {
            a::b::c = 10;
            a::b::c;
        }
        w: i32 = z();
        """);

    var code = console.Prompt(new TextPrompt<string>(">").DefaultValue(@default));

    if (code == @default)
        code = Markup.Remove(code);

    var compilation = Compilation.Compile(new SourceText(code), previousCompilation);

    if (compilation.Diagnostics.Count > 0)
    {
        foreach (var diagnostic in compilation.Diagnostics)
            console.WriteLine(diagnostic);
        if (compilation.Diagnostics.HasErrorDiagnostics)
            continue;
    }

    foreach (var boundTree in compilation.BoundTrees)
    {
        //console.WriteLine(boundTree.SyntaxTree);
        console.WriteLine(boundTree);
    }

    var evaluation = Evaluation.Evaluate(compilation, previousEvaluation);

    console.WriteLine(evaluation.Values[0]);

    previousCompilation = compilation;
    previousEvaluation = evaluation;
}
