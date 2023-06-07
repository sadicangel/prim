using CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System.Text;

var variables = new Dictionary<Variable, object>();
var showTree = false;
var showProgram = false;
var builder = new StringBuilder();
var previousCompilation = default(Compilation);

while (true)
{
    Console.Out.WriteColored(builder.Length == 0 ? "» " : "· ", ConsoleColor.Green);

    var input = Console.ReadLine();

    if (builder.Length == 0)
    {
        switch (input)
        {
            case "\\tree":
                showTree = true;
                Console.Out.WriteLineColored($"[{(showTree ? "showing" : "hiding")} parse trees]", ConsoleColor.DarkGray);
                continue;

            case "\\program":
                showProgram = true;
                Console.Out.WriteLineColored($"[{(showProgram ? "showing" : "hiding")} bound tree]", ConsoleColor.DarkGray);
                continue;

            case "\\cls":
                Console.Clear();
                continue;

            case "\\reset":
                previousCompilation = null;
                continue;

            case "\\exit":
                return;
        }
    }

    builder.AppendLine(input);
    var syntaxTree = SyntaxTree.Parse(builder.ToString());

    if (!String.IsNullOrWhiteSpace(input) && syntaxTree.Diagnostics.Any())
        continue;

    var compilation = new Compilation(syntaxTree, previousCompilation);
    var result = compilation.Evaluate(variables);

    var diagnostics = result.Diagnostics;

    if (showTree)
    {
        syntaxTree.WriteTo(Console.Out);
    }

    if (showProgram)
    {
        compilation.WriteTo(Console.Out);
    }

    Console.ResetColor();

    if (!diagnostics.Any())
    {
        Console.Out.WriteLineColored(result.Value, ConsoleColor.Magenta);
        previousCompilation = compilation;
    }
    else
    {
        foreach (var diagnostic in diagnostics)
        {
            var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
            var lineNumber = lineIndex + 1;
            var line = syntaxTree.Text.Lines[lineIndex];
            var columnIndex = diagnostic.Span.Start - line.Start;
            var columnNumber = columnIndex + 1;
            var diagnosticColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;

            Console.WriteLine();

            Console.Out.WriteLineColored($"({lineNumber}, {columnNumber}): {diagnostic}", diagnosticColor);

            var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
            var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

            var prefix = syntaxTree.Text[prefixSpan];
            var error = syntaxTree.Text[diagnostic.Span];
            var suffix = syntaxTree.Text[suffixSpan];

            Console.Write("    ");
            Console.Write(prefix.ToString());
            Console.Out.WriteColored(error.ToString(), diagnosticColor);
            Console.Write(suffix.ToString());
            Console.WriteLine();

            Console.Write(new string(' ', 4 + columnIndex));
            Console.Write(new string('˄', error.Length));
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    builder.Clear();
}