using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Symbols;

public abstract record class Symbol(string Name, SymbolKind Kind) : INode
{
    TextSpan INode.Span { get; }

    IEnumerable<INode> INode.GetChildren() => Enumerable.Empty<INode>();
    void INode.WriteTo(TextWriter writer, string indent, bool isLast)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored((object)indent, ConsoleColor.DarkGray);
        writer.WriteColored((object)marker, ConsoleColor.DarkGray);
        writer.WriteColored(Name, ConsoleColor.Cyan);
        writer.WriteLine();
    }

    public override string ToString() => Name;

    public static SymbolKind GetKind<T>()
    {
        switch (typeof(T))
        {
            case Type t when t == typeof(FunctionSymbol):
                return SymbolKind.Function;
            case Type t when t == typeof(ParameterSymbol):
                return SymbolKind.Parameter;
            case Type t when t == typeof(TypeSymbol):
                return SymbolKind.Type;
            case Type t when t == typeof(VariableSymbol):
                return SymbolKind.Variable;
            default:
                throw new InvalidOperationException($"Type '{typeof(T)}' is not a {nameof(Symbol)}");
        }
    }
}