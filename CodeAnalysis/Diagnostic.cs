using CodeAnalysis.Text;

namespace CodeAnalysis;

public sealed record class Diagnostic(bool IsError, TextSpan Span, string Message)
{
    public override string ToString() => Message;
}