using CodeAnalysis.Text;

namespace CodeAnalysis;

public sealed record class Diagnostic(bool IsError, TextLocation Location, string Message)
{
    public override string ToString() => Message;
}