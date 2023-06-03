namespace CodeAnalysis.Text;

public readonly record struct TextSpan(int Start, int Length)
{
    public int End { get => Start + Length; }
    public Range Range => Start..(Start + Length);

    public static TextSpan FromBounds(int start, int end) => new(start, end - start);

    public static implicit operator Range(TextSpan span) => span.Range;

    public static TextSpan operator -(TextSpan left, TextSpan right) => FromBounds(left.Start, right.End);

}