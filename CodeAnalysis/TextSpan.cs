namespace CodeAnalysis;

public readonly record struct TextSpan(int Start, int Length)
{
    public int End { get => Start + Length; }
    public Range Range => Start..(Start + Length);

    public static implicit operator Range(TextSpan span) => span.Range;
}
