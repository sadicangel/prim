namespace CodeAnalysis.Text;
public sealed record class TextLocation(SourceText Text, TextSpan Span)
{
    public int StartLine => Text.GetLineIndex(Span.Start);
    public int StartCharacter => Span.Start - Text.Lines[StartLine].Start;
    public int EndLine => Text.GetLineIndex(Span.End);
    public int EndCharacter => Span.End - Text.Lines[EndLine].End;
}
