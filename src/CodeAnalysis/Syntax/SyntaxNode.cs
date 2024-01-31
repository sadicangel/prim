using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class SyntaxNode(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : INode
{
    //public SyntaxNode? Parent => SyntaxTree.GetParent(this);

    public Token FirstToken => this is Token token ? token : Children().First().FirstToken;
    public Token LastToken => this is Token token ? token : Children().Last().LastToken;


    public virtual Range Range => Children().First().Range.Start..Children().Last().Range.End;
    public virtual Range RangeWithWhiteSpace => Children().First().RangeWithWhiteSpace.Start..Children().Last().RangeWithWhiteSpace.End;
    public virtual ReadOnlySpan<char> Text => SyntaxTree.Source[Range];
    public SourceLocation Location => new(SyntaxTree.Source, Range);
    public override string ToString() => Text.ToString();

    public abstract IEnumerable<SyntaxNode> Children();
    IEnumerable<INode> INode.Children() => Children();
}

public enum SyntaxNodeKind
{
    GlobalExpression,

    BlockExpression,
    InlineExpression,
    EmptyExpression,

    LiteralExpression,
    GroupExpression,
    UnaryExpression,
    BinaryExpression,
    ConvertExpression,

    DeclarationExpression,
    AssignmentExpression,
    NameExpression,

    ForExpression,
    IfElseExpression,
    WhileExpression,

    BreakExpression,
    ContinueExpression,
    ReturnExpression,

    // Token
    Token,

    // Trivia
    Trivia,

    // Nodes
    Type,
    Parameter,
    ArgumentList,
    CompilationUnit,
    SubscriptExpression,
}