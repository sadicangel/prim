namespace CodeAnalysis.Text;
public sealed record class SourceText(string Text, string FilePath)
{
    private ReadOnlyList<SourceLine>? _lines;

    public SourceText(string text) : this(text, string.Empty) { }

    public string FileName { get; } = String.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileName(FilePath);

    public int Length { get => Text.Length; }

    public ReadOnlyList<SourceLine> Lines { get => _lines ??= ParseLines(Text); }

    public char this[Index index]
    {
        get
        {
            var offset = index.GetOffset(Length);
            if (offset < 0 || offset >= Length)
                return '\0';
            return Text[offset];
        }
    }

    public ReadOnlySpan<char> this[Range range] => Text.AsSpan(range);

    public int GetLineIndex(Index position)
    {
        var lower = 0;
        var upper = Lines.Count - 1;

        while (lower <= upper)
        {
            var index = lower + (upper - lower) / 2;
            var start = Lines[index].Range.Start;

            if (position.Value == start.Value)
                return index;

            if (start.Value > position.Value)
                upper = index - 1;
            else
                lower = index + 1;
        }

        return lower - 1;
    }

    public override string ToString() => Text;

    private ReadOnlyList<SourceLine> ParseLines(ReadOnlySpan<char> text)
    {
        var lines = new List<SourceLine>();

        var position = 0;
        var lineStart = 0;
        while (position < text.Length)
        {
            var lineBreakWidth = text[position..] switch
            {
            ['\r', '\n', ..] => 2,
            ['\r', ..] or ['\n', ..] => 1,
                _ => 0,
            };

            if (lineBreakWidth == 0)
            {
                position++;
            }
            else
            {
                lines.Add(new SourceLine(this, lineStart..position, lineStart..(position + lineBreakWidth)));
                position += lineBreakWidth;
                lineStart = position;
            }
        }

        if (position >= lineStart)
            lines.Add(new SourceLine(this, lineStart..position, lineStart..position));

        return new(lines);
    }
}
