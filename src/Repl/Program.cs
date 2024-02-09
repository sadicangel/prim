using CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using Spectre.Console;
using System.Text;

Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

var syntaxTree = SyntaxTree.ParseScript("""
    a: f32 = 10;
    println(a);
    """);

if (syntaxTree.Diagnostics.Count > 0)
{
    foreach (var diagnostic in syntaxTree.Diagnostics)
        AnsiConsole.Write(diagnostic.ToRenderable());
}
else
{
    AnsiConsole.Write(syntaxTree.Root.Text.ToString());
    AnsiConsole.WriteLine();
    AnsiConsole.WriteLine();

    AnsiConsole.Write(syntaxTree.ToRenderable());
    AnsiConsole.WriteLine();

    var compilation = new Compilation([syntaxTree]);

    var result = compilation.Evaluate();
    if (result.Diagnostics.Count > 0)
    {
        foreach (var diagnostic in result.Diagnostics)
            AnsiConsole.Write(diagnostic.ToRenderable());
    }
    else
    {
        var value = result.Type switch
        {
            FunctionType function => function.Name,
            _ => result.Value
        };
        AnsiConsole.Write(new Markup($"{value} ", "grey66"));
        AnsiConsole.Write(new Markup(result.Type.ToString(), "green i"));
        AnsiConsole.WriteLine();
    }
}
