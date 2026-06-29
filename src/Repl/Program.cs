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
        demo.showcase :: module;

        Point := type {
            x: i32 = 0;
            y: i32 = 0;
            label: str = "origin";
        };

        FeatureBag := type {
            maybeName: str? = null;
            tags: str[] = ["parser", "binder"];
            score: i32 | bool = 1;
            next: Point* = null;
            project: (Point) -> i32 = (p) => p.x + p.y;
        };

        origin: Point = Point { x = 2, y = 3, label = "origin" };
        shifted: Point = move(origin, 4);
        qualified: demo.showcase.Point = demo.showcase.Point { x = 8, y = 13, label = "qualified" };
        points: Point[] = [origin, shifted, qualified];

        firstX: i32 = points[0].x;
        wide: i64 = firstX as i64;
        mathy: i32 = -(firstX + 10) * 2 / 3 % 4;
        bits: i32 = (1 << 3) | (2 & 3) ^ ~0;
        ready: bool = !false && (firstX < 10 || shifted.x >= qualified.x);
        message: str = "point: " + origin.label;
        maybePoint: Point | unit = if (ready) shifted else null;

        choose: (bool, Point, Point) -> Point = (useLeft, left, right) =>
            if (useLeft) left else right;

        move: (Point, i32) -> Point = (p, delta) => {
            next: Point = Point {
                x = p.x + delta,
                y = p.y + delta,
                label = p.label + " moved"
            };
            return next
        };

        sumUntil: (i32) -> i32 = (limit) => {
            i: i32 = 0;
            total: i32 = 0;
            while (i < limit) {
                i = i + 1;
                if (i == 3) continue;
                total = total + i;
                if (total > 10) break total;
            };
            return total
        };

        summary: i32 = sumUntil(8) + choose(true, origin, shifted).x;
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

    var (declarations, bindDiagnostics) = compilation.Bind();

    if (bindDiagnostics.Length > 0)
    {
        foreach (var diagnostic in bindDiagnostics)
            console.WriteLine(diagnostic);
        if (bindDiagnostics.HasErrorDiagnostics)
            continue;
    }

    console.Clear(true);

    foreach (var syntaxTree in compilation.SyntaxTrees)
    {
        console.WriteLine(syntaxTree);
    }

    console.MarkupLine($"[green]Bound {declarations.Length} declaration(s).[/]");
}