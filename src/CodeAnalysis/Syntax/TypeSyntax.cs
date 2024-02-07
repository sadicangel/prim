using CodeAnalysis.Types;
using System.Text;

namespace CodeAnalysis.Syntax;

public sealed record class TypeSyntax(
    SyntaxTree SyntaxTree,
    IReadOnlyList<Token> Tokens,
    PrimType Type
)
    : SyntaxNode(SyntaxNodeKind.Type, SyntaxTree)
{
    public TypeSyntax(SyntaxTree syntaxTree) : this(syntaxTree, [], PredefinedTypes.Unit) { }

    public override IEnumerable<SyntaxNode> Children() => Tokens;

    public override void WriteMarkupTo(StringBuilder builder)
    {
        foreach (var token in Tokens)
            builder.Token(token);
    }
}
