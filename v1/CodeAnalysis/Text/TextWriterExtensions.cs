namespace CodeAnalysis.Text;

public static class TextWriterExtensions
{
    public static void WriteColored(this TextWriter writer, string? text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        writer.Write(text);
        Console.ResetColor();
    }
    public static void WriteColored(this TextWriter writer, ReadOnlySpan<char> text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        writer.Write(text);
        Console.ResetColor();
    }

    public static void WriteColored(this TextWriter writer, object? value, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        writer.Write(value);
        Console.ResetColor();
    }

    public static void WriteLineColored(this TextWriter writer, string? text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        writer.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteLineColored(this TextWriter writer, ReadOnlySpan<char> text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        writer.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteLineColored(this TextWriter writer, object? value, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        writer.WriteLine(value);
        Console.ResetColor();
    }
}
