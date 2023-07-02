using CodeAnalysis.Syntax;

namespace CodeAnalysis.Symbols;

public abstract record class Symbol(SymbolKind SymbolKind, string Name, TypeSymbol Type) : INode
{
    IEnumerable<INode> INode.GetChildren() => Enumerable.Empty<INode>();

    void INode.WriteTo(TextWriter writer, string indent, bool isLast)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored((object)indent, ConsoleColor.DarkGray);
        writer.WriteColored((object)marker, ConsoleColor.DarkGray);
        writer.WriteColored(Name, ConsoleColor.Cyan);
        writer.WriteLine();
    }

    public static SymbolKind GetKind<T>()
    {
        switch (typeof(T))
        {
            case Type t when t == typeof(FunctionSymbol):
                return SymbolKind.Function;
            case Type t when t == typeof(LabelSymbol):
                return SymbolKind.Label;
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