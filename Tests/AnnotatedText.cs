using CodeAnalysis.Text;
using System.Text;

namespace CodeAnalysis;

internal sealed record class AnnotatedText(string Text, IReadOnlyList<TextSpan> Spans)
{
    public static AnnotatedText Parse(string text)
    {
        var builder = new StringBuilder();
        var spans = new List<TextSpan>();
        var startStack = new Stack<int>();

        var i = 0;

        foreach (var c in text)
        {
            switch (c)
            {
                case '⟨':
                    startStack.Push(i);
                    break;
                case '⟩' when startStack.TryPop(out var start):
                    spans.Add(TextSpan.FromBounds(start, end: i));
                    break;
                case '⟩':
                    throw new ArgumentException("Too many ']' in text", nameof(text));
                default:
                    builder.Append(c);
                    ++i;
                    break;
            }
        }

        if (startStack.Count > 0)
            throw new ArgumentException("Missing ']' in text", nameof(text));

        return new AnnotatedText(builder.ToString(), spans);
    }
}