using CodeAnalysis;
using CodeAnalysis.Syntax;

var variables = new Dictionary<Variable, object>();

var showTree = false;

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (String.IsNullOrWhiteSpace(line))
        return;

    if (line.StartsWith('\\'))
    {
        switch (line)
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

    var syntaxTree = SyntaxTree.Parse(line);
    var compilation = new Compilation(syntaxTree);
    var result = compilation.Evaluate(variables);

    var diagnostics = result.Diagnostics;

    if (showTree)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        syntaxTree.PrettyPrint(Console.Out);
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
            Console.WriteLine();
            Console.ForegroundColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
            Console.WriteLine(diagnostic);
            Console.ResetColor();

            var prefix = line[..diagnostic.Span.Start];
            var error = line[diagnostic.Span.Range];
            var suffix = line[diagnostic.Span.End..];

            Console.Write("    ");
            Console.Write(prefix);

            Console.ForegroundColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
            Console.Write(error);
            Console.ResetColor();

            Console.Write(suffix);
            Console.WriteLine();
        }
        Console.WriteLine();

        Console.ResetColor();
    }
}