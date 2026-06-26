using System.Collections.Immutable;
using CodeAnalysis;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Text;
using Repl;
using Spectre.Console;

var console = AnsiConsole.Console;

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

    var compilation = new Compilation(new SourceText(code));

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
}
