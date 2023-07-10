using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class Parameter(SyntaxTree SyntaxTree, Token Identifier, Token Colon, Token Type)
    : SyntaxNode(SyntaxNodeKind.Parameter, SyntaxTree)
{
    public override TextSpan Span { get => Identifier.Span; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return Colon;
        yield return Type;
    }
}

public sealed record class FunctionDeclaration(SyntaxTree SyntaxTree, Token Modifier, Token Identifier, Token Colon, Token OpenParenthesis, SeparatedNodeList<Parameter> Parameters, Token CloseParenthesis, Token Arrow, Token Type, Token Equal, BlockStatement Body, Token Semicolon)
    : Declaration(SyntaxTree)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Modifier;
        yield return Identifier;
        yield return Colon;
        yield return OpenParenthesis;
        foreach (var node in Parameters.Nodes)
            yield return node;
        yield return CloseParenthesis;
        yield return Arrow;
        yield return Type;
        yield return Equal;
        foreach (var node in Body.GetChildren())
            yield return node;
        yield return Semicolon;
    }
}