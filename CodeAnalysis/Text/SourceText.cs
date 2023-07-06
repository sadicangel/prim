using System.Collections;

namespace CodeAnalysis.Text;

public sealed record class SourceText(string Text, string FileName = "") : IReadOnlyList<char>
{
    private IReadOnlyList<TextLine>? _lines;
    public IReadOnlyList<TextLine> Lines { get => _lines ??= ParseLines(Text); }

    public int Length { get => Text.Length; }
    int IReadOnlyCollection<char>.Count { get => Text.Length; }

    public char this[int index] { get => Text[index]; }

    public ReadOnlySpan<char> this[TextSpan span] { get => Text.AsSpan()[(Range)span]; }

    public ReadOnlySpan<char> this[Range range] { get => Text.AsSpan()[range]; }

    public ReadOnlySpan<char> Slice(int start, int length) => Text.AsSpan(start, length);

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

    public override string ToString() => Text.ToString();

    public IEnumerator<char> GetEnumerator()
    {
        for (int i = 0; i < Text.Length; ++i)
            yield return Text[i];

    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private IReadOnlyList<TextLine> ParseLines(ReadOnlySpan<char> text)
    {
        var lines = new List<TextLine>();

        var position = 0;
        var lineStart = 0;
        while (position < text.Length)
        {
            var lineBreakWidth = GetLineBreakWidth(text, position);
            if (lineBreakWidth == 0)
            {
                position++;
            }
            else
            {
                lines.Add(CreateLine(position, lineStart, lineBreakWidth));
                position += lineBreakWidth;
                lineStart = position;
            }
        }

        if (position >= lineStart)
            lines.Add(CreateLine(position, lineStart, 0));

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

        TextLine CreateLine(int position, int lineStart, int lineBreakWidth)
        {
            var lineLength = position - lineStart;
            var lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
            return new TextLine(this, lineStart, lineLength, lineLengthIncludingLineBreak);
        }
    }
}