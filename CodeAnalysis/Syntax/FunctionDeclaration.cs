namespace CodeAnalysis.Syntax;

public sealed record class FunctionDeclaration(SyntaxTree SyntaxTree, Token Modifier, Token Identifier, Token Colon, Token OpenParenthesis, SeparatedNodeList<Parameter> Parameters, Token CloseParenthesis, Token Arrow, Token Type, Token Equal, BlockStatement Body, Token Semicolon)
    : Declaration(DeclarationKind.Function, SyntaxTree)
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