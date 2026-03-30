using System.Collections.Immutable;
using CodeAnalysis;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

var parseOptions = new ParseOptions { IsScript = true };
var previousCompilation = default(Compilation);
//var previousEvaluation = default(Evaluation);

while (true)
{
    var @default = Markup.Escape(
        """
        module vec;

        struct Point {
            x: i32 = 0;
            y: i32 = 0;
        }

        let value: i32 = 0;

        let main: (str[]) -> i32 = (args) => {
            var a = 40;
            var b = -2;
            let c = a + b;
        }
        """);

    var code = console.Prompt(new TextPrompt<string>(">").DefaultValue(@default));

    if (code == @default)
        code = Markup.Remove(code);

    //if (code.StartsWith('#'))
    //{
    //    switch (code)
    //    {
    //        case "#scope" when previousCompilation is not null:
    //            console.WriteLine(previousCompilation.BoundScope);
    //            break;
    //    }
    //    continue;
    //}

    var compilation = new Compilation(new SourceText(code), parseOptions, previousCompilation);

    var parseDiagnostics = compilation.GetDiagnostics().ToImmutableArray();

    if (parseDiagnostics.Length > 0)
    {
        foreach (var diagnostic in parseDiagnostics)
            console.WriteLine(diagnostic);
        if (parseDiagnostics.HasErrorDiagnostics)
            continue;
    }

    console.Clear(true);

    foreach (var syntaxTree in compilation.SyntaxTrees)
    {
        console.WriteLine(syntaxTree);
    }

    var (boundNode, bindDiagnostics) = compilation.Bind(compilation.EntryPoint!);

    if (bindDiagnostics.Length > 0)
    {
        foreach (var diagnostic in bindDiagnostics)
            console.WriteLine(diagnostic);
        if (bindDiagnostics.HasErrorDiagnostics)
            continue;
    }

    console.WriteLine(boundNode);

    //foreach (var boundTree in compilation.BoundTrees)
    //{
    //    console.WriteLine(boundTree);
    //}

    //var evaluation = Evaluation.Evaluate(compilation, previousEvaluation);

    //console.WriteLine(evaluation.Values[0]);

    previousCompilation = compilation;
    //previousEvaluation = evaluation;
}
