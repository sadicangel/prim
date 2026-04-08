using System.Collections.Immutable;
using CodeAnalysis;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Evaluation;
using CodeAnalysis.Evaluation.Values;
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
        let main: (str[]) -> i32 = (args) => {
            var i = 0;
            var j = 0;
            while (i < 10) {
                i = i + 1;
                if (i < 5) continue;
                break i;
            }
            j = i * 2;

            return j;
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

    var evaluation = new Interpreter().Interpret(boundNode);

    var result = (PrimValue)((LambdaValue)evaluation).Delegate.DynamicInvoke(default(ArrayValue)!)!;
    console.WriteLine(result);

    previousCompilation = compilation;
    //previousEvaluation = evaluation;
}
