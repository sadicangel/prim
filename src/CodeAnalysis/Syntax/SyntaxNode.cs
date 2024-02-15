using CodeAnalysis.Text;
using System.Text;

namespace CodeAnalysis.Syntax;

public abstract record class SyntaxNode(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : INode
{
    public SyntaxNode? Parent => SyntaxTree.GetParent(this);

    public Token FirstToken => this is Token token ? token : Children().First().FirstToken;
    public Token LastToken => this is Token token ? token : Children().Last().LastToken;


    public virtual Range Range => Children().First().Range.Start..Children().Last().Range.End;
    public virtual Range RangeWithWhiteSpace => Children().First().RangeWithWhiteSpace.Start..Children().Last().RangeWithWhiteSpace.End;
    public virtual ReadOnlySpan<char> Text => SyntaxTree.Source[Range];
    public SourceLocation Location => new(SyntaxTree.Source, Range);

    public abstract IEnumerable<SyntaxNode> Children();
    IEnumerable<INode> INode.Children() => Children();

    public sealed override string ToString() => $"{NodeKind} {SyntaxTree.Source[Range]}";

    public abstract void WriteMarkupTo(StringBuilder builder);
}

public enum SyntaxNodeKind
{
    GlobalDeclarationExpression,

    BlockExpression,
    InlineExpression,
    EmptyExpression,
    LocalDeclarationExpression,

    LiteralExpression,
    GroupExpression,
    UnaryExpression,
    BinaryExpression,
    LambdaExpression,

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
    Operator,
    Parameter,
    ArgumentList,
    CompilationUnit,
}