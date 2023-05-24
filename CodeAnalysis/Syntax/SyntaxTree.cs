namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(IEnumerable<Diagnostic> Diagnostics, Expression Root, Token Eof) : IPrintableNode
{
    public void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true) => Root.PrettyPrint(writer, indent, isLast);
    public IEnumerable<IPrintableNode> GetChildren() => Root.GetChildren();

    public static SyntaxTree Parse(string text) => new Parser(text).Parse();
}