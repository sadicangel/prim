
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class GlobalExpression(
    SyntaxTree SyntaxTree,
    DeclarationExpression Declaration,
    Token Semicolon
)
    : Expression(SyntaxNodeKind.GlobalExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children() => throw new NotImplementedException();
}
