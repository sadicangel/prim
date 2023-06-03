using CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System.Text;

var variables = new Dictionary<Variable, object>();

var showTree = false;

var builder = new StringBuilder();

while (true)
{
    Console.Write(builder.Length == 0 ? "> " : "| ");

    var input = Console.ReadLine();

    if (builder.Length == 0)
    {
        switch (input)
        {
            case "\\showtree":
                showTree = true;
                Console.WriteLine($"<{(showTree ? "showing" : "hiding")} parse trees>");
                continue;

            case "\\cls":
                Console.Clear();
                continue;
        }
    }

    builder.AppendLine(input);
    var syntaxTree = SyntaxTree.Parse(builder.ToString());

    if (!String.IsNullOrWhiteSpace(input) && syntaxTree.Diagnostics.Any())
        continue;

    var compilation = new Compilation(syntaxTree);
    var result = compilation.Evaluate(variables);

    var diagnostics = result.Diagnostics;

    if (showTree)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        syntaxTree.WriteTo(Console.Out);
    }

    Console.ResetColor();

    if (!diagnostics.Any())
    {
        Console.WriteLine(result.Value);
    }
    else
    {
        foreach (var diagnostic in diagnostics)
        {
            var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
            var lineNumber = lineIndex + 1;
            var line = syntaxTree.Text.Lines[lineIndex];
            var character = diagnostic.Span.Start - line.Start + 1;

            Console.WriteLine();
            Console.ForegroundColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
            Console.Write($"({lineNumber}, {character}): ");
            Console.WriteLine(diagnostic);
            Console.ResetColor();

            var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
            var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

            var prefix = syntaxTree.Text[prefixSpan];
            var error = syntaxTree.Text[diagnostic.Span];
            var suffix = syntaxTree.Text[suffixSpan];

            Console.Write("    ");
            Console.Write(prefix.ToString());

            Console.ForegroundColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
            Console.Write(error.ToString());
            Console.ResetColor();

            Console.Write(suffix.ToString());
            Console.WriteLine();
        }
        Console.WriteLine();

        Console.ResetColor();
    }

    builder.Clear();
}