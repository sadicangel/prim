﻿using CodeAnalysis;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

var previousCompilation = default(Compilation);
var previousEvaluation = default(Evaluation);

while (true)
{
    var code = console.Prompt(new TextPrompt<string>(">").DefaultValue("""
        Value: struct = {
            i: i32 = 0;
            get_i: () -> i32 = this.i;
        }

        v: Value = Value { .i = 10 }
        a: i32 = v.get_i();
        a = v.get_i()
        a
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
        console.WriteLine(boundTree);

    var evaluation = Evaluation.Evaluate(compilation, previousEvaluation);

    console.WriteLine(evaluation.Values[0]);

    previousCompilation = compilation;
    previousEvaluation = evaluation;
}
