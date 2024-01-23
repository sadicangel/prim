namespace CodeAnalysis.Syntax.Statements;

public abstract record class Declaration(SyntaxTree SyntaxTree) : Statement(SyntaxNodeKind.DeclarationStatement, SyntaxTree)
{
    public override string ToString() => base.ToString();
}
