using System.Collections;

namespace CodeAnalysis.Text;

public sealed record class SourceText : IReadOnlyList<char>
{
    private readonly ReadOnlyMemory<char> _text;

    public SourceText(ReadOnlyMemory<char> text)
    {
        _text = text;
        Lines = ParseLines(this, text);
    }

    public IReadOnlyList<TextLine> Lines { get; }

    public int Length { get => _text.Length; }
    int IReadOnlyCollection<char>.Count { get => _text.Length; }

    public char this[int index] { get => _text.Span[index]; }

    public ReadOnlySpan<char> this[TextSpan span] { get => _text.Span[(Range)span]; }

    public ReadOnlySpan<char> this[Range range] { get => _text.Span[range]; }

    public ReadOnlySpan<char> Slice(int start, int length) => _text.Span.Slice(start, length);

    public int GetLineIndex(int position)
    {
        var lower = 0;
        var upper = Lines.Count - 1;

        while (lower <= upper)
        {
            var index = lower + (upper - lower) / 2;
            var start = Lines[index].Start;

            if (position == start)
                return index;

            if (start > position)
            {
                upper = index - 1;
            }
            else
            {
                lower = index + 1;
            }
        }

        return lower - 1;
    }

    public override string ToString() => _text.ToString();

    public IEnumerator<char> GetEnumerator()
    {
        for (int i = 0; i < _text.Length; ++i)
            yield return _text.Span[i];

    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static IReadOnlyList<TextLine> ParseLines(SourceText sourceText, ReadOnlyMemory<char> text)
    {
        var lines = new List<TextLine>();

        var position = 0;
        var lineStart = 0;
        while (position < text.Length)
        {
            var lineBreakWidth = GetLineBreakWidth(text.Span, position);
            if (lineBreakWidth == 0)
            {
                position++;
            }
            else
            {
                lines.Add(CreateLine(sourceText, position, lineStart, lineBreakWidth));
                position += lineBreakWidth;
                lineStart = position;
            }
        }

        if (position >= lineStart)
            lines.Add(CreateLine(sourceText, position, lineStart, 0));

        return lines;

        static int GetLineBreakWidth(ReadOnlySpan<char> text, int i)
        {
            return text[i..] switch
            {
                ['\r', '\n', ..] => 2,
                ['\r', ..] or ['\n', ..] => 1,
                _ => 0,
            };
        }

        static TextLine CreateLine(SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            var lineLength = position - lineStart;
            var lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
            return new TextLine(sourceText, lineStart, lineLength, lineLengthIncludingLineBreak);
        }
    }
}