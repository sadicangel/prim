namespace CodeAnalysis.Text;

public sealed record class TextLine
{
    private readonly SourceText _sourceText;

    public TextLine(SourceText sourceText, int start, int length, int lengthIncludingLineBreak)
    {
        _sourceText = sourceText;
        Start = start;
        Length = length;
        LengthIncludingLineBreak = lengthIncludingLineBreak;
    }

    public int Start { get; }
    public int Length { get; }
    public int End { get => Start + Length; }
    public int LengthIncludingLineBreak { get; }
    public TextSpan Span { get => new(Start, Length); }
    public TextSpan SpanIncludingLineBreak { get => new(Start, LengthIncludingLineBreak); }
    public ReadOnlySpan<char> Text { get => _sourceText[Span]; }

    public override string ToString() => Text.ToString();
}