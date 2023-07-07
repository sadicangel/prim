namespace CodeAnalysis.Text;
public sealed record class TextLocation(SourceText Text, TextSpan Span) : IComparable<TextLocation>
{
    public string FileName { get => Text.FileName; }
    public int StartLine => Text.GetLineIndex(Span.Start);
    public int StartCharacter => Span.Start - Text.Lines[StartLine].Start;
    public int EndLine => Text.GetLineIndex(Span.End);
    public int EndCharacter => Span.End - Text.Lines[StartLine].Start;

    public int CompareTo(TextLocation? other)
    {
        if (other is null)
            return 1;

        var cmp = Text.FileName.CompareTo(other.Text.FileName);
        if (cmp != 0)
            return cmp;

        return Span.CompareTo(other.Span);
    }

    public static bool operator <(TextLocation left, TextLocation right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(TextLocation left, TextLocation right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(TextLocation left, TextLocation right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(TextLocation left, TextLocation right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
