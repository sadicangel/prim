namespace CodeAnalysis.Text;
public sealed record class Source(string Text, string FilePath)
{
    private IReadOnlyList<SourceLine>? _lines;

    public string FileName { get; } = String.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileName(FilePath);

    public IReadOnlyList<SourceLine> Lines { get => _lines ??= ParseLines(Text); }

    public char this[Index index] => index.GetOffset(Text.Length) < 0 || index.GetOffset(Text.Length) >= Text.Length ? '\0' : Text[index];

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

    private List<SourceLine> ParseLines(ReadOnlySpan<char> text)
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

        return lines;
    }

    public static implicit operator Source(string text) => new(text, string.Empty);
}
