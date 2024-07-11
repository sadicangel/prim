using System.Collections;
using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;

internal class BoundScope(BoundScope? parent = null) : IEnumerable<Symbol>
{
    protected Dictionary<string, Symbol>? Symbols { get; set; }

    public BoundScope Parent { get => parent ?? GlobalBoundScope.Instance; }

    public bool Declare(Symbol symbol) => (Symbols ??= []).TryAdd(symbol.Name, symbol);

    public void Replace(Symbol symbol)
    {
        var scope = this;
        if (scope.Symbols?.ContainsKey(symbol.Name) is true)
        {
            scope.Symbols[symbol.Name] = symbol;
            return;
        }

        do
        {
            scope = scope.Parent;
            if (scope.Symbols?.ContainsKey(symbol.Name) is true)
            {
                scope.Symbols[symbol.Name] = symbol;
                return;
            }
        }
        while (scope != scope.Parent);

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public Symbol? Lookup(string name)
    {
        var scope = this;
        var symbol = scope.Symbols?.GetValueOrDefault(name);
        if (symbol is not null)
            return symbol;

        do
        {
            scope = scope.Parent;
            symbol = scope.Symbols?.GetValueOrDefault(name);
            if (symbol is not null)
                return symbol;
        }
        while (scope != scope.Parent);

        return null;
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
        foreach (var symbol in EnumerateSymbols(this))
            yield return symbol;

        static IEnumerable<Symbol> EnumerateSymbols(BoundScope? scope)
        {
            if (scope is null) yield break;
            if (scope.Symbols is not null)
            {
                foreach (var (_, symbol) in scope.Symbols)
                    yield return symbol;
            }
            foreach (var symbol in EnumerateSymbols(scope.Parent))
                yield return symbol;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class GlobalBoundScope : BoundScope
{
    private static GlobalBoundScope? s_instance;
    public static GlobalBoundScope Instance
    {
        get
        {
            if (s_instance is null)
                Interlocked.CompareExchange(ref s_instance, new GlobalBoundScope(), null);
            return s_instance;
        }
    }

    private GlobalBoundScope() : base()
    {
        Symbols = new Dictionary<string, Symbol>()
        {
            [PredefinedTypeNames.Any] = Any = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.AnyKeyword), PredefinedTypes.Any, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.Unknown] = Unknown = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.UnknownKeyword), PredefinedTypes.Unknown, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.Never] = Never = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.NeverKeyword), PredefinedTypes.Never, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.Unit] = Unit = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.UnitKeyword), PredefinedTypes.Unit, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.Type] = Type = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.TypeKeyword), PredefinedTypes.Type, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.Str] = Str = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.StrKeyword), PredefinedTypes.Str, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.Bool] = Bool = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.BoolKeyword), PredefinedTypes.Bool, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.I8] = I8 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I8Keyword), PredefinedTypes.I8, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.I16] = I16 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I16Keyword), PredefinedTypes.I16, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.I32] = I32 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I32Keyword), PredefinedTypes.I32, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.I64] = I64 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I64Keyword), PredefinedTypes.I64, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.I128] = I128 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I128Keyword), PredefinedTypes.I128, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.ISize] = ISize = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.ISizeKeyword), PredefinedTypes.ISize, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.U8] = U8 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U8Keyword), PredefinedTypes.U8, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.U16] = U16 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U16Keyword), PredefinedTypes.U16, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.U32] = U32 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U32Keyword), PredefinedTypes.U32, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.U64] = U64 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U64Keyword), PredefinedTypes.U64, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.U128] = U128 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U128Keyword), PredefinedTypes.U128, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.USize] = USize = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.USizeKeyword), PredefinedTypes.USize, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.F16] = F16 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F16Keyword), PredefinedTypes.F16, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.F32] = F32 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F32Keyword), PredefinedTypes.F32, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.F64] = F64 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F64Keyword), PredefinedTypes.F64, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.F80] = F80 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F80Keyword), PredefinedTypes.F80, NamespaceSymbol.Global, NamespaceSymbol.Global),
            [PredefinedTypeNames.F128] = F128 = new TypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F128Keyword), PredefinedTypes.F128, NamespaceSymbol.Global, NamespaceSymbol.Global),
        };
    }

    public TypeSymbol Any { get; }
    public TypeSymbol Unknown { get; }
    public TypeSymbol Never { get; }
    public TypeSymbol Unit { get; }
    public TypeSymbol Type { get; }
    public TypeSymbol Str { get; }
    public TypeSymbol Bool { get; }
    public TypeSymbol I8 { get; }
    public TypeSymbol I16 { get; }
    public TypeSymbol I32 { get; }
    public TypeSymbol I64 { get; }
    public TypeSymbol I128 { get; }
    public TypeSymbol ISize { get; }
    public TypeSymbol U8 { get; }
    public TypeSymbol U16 { get; }
    public TypeSymbol U32 { get; }
    public TypeSymbol U64 { get; }
    public TypeSymbol U128 { get; }
    public TypeSymbol USize { get; }
    public TypeSymbol F16 { get; }
    public TypeSymbol F32 { get; }
    public TypeSymbol F64 { get; }
    public TypeSymbol F80 { get; }
    public TypeSymbol F128 { get; }
}
