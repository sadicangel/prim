namespace CodeAnalysis.Text;

public readonly record struct TextSpan(int Start, int Length) : IComparable<TextSpan>
{
    public int End { get => Start + Length; }
    public Range Range => Start..(Start + Length);

    public static TextSpan FromBounds(int start, int end) => new(start, end - start);
    public static TextSpan FromBounds(TextSpan start, TextSpan end) => FromBounds(start.Start, end.End);

    public static implicit operator Range(TextSpan span) => span.Range;

    public override string ToString() => Range.ToString();
    public int CompareTo(TextSpan other)
    {
        var cmp = Start - other.Start;
        return cmp != 0 ? cmp : Length - other.Length;
    }

    public static bool operator <(TextSpan left, TextSpan right) => left.CompareTo(right) < 0;

    public static bool operator <=(TextSpan left, TextSpan right) => left.CompareTo(right) <= 0;

    public static bool operator >(TextSpan left, TextSpan right) => left.CompareTo(right) > 0;

    public static bool operator >=(TextSpan left, TextSpan right) => left.CompareTo(right) >= 0;
}