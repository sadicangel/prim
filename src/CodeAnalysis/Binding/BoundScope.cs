using System.Collections;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;

internal class BoundScope(BoundScope? parent = null) : IEnumerable<Symbol>
{
    private static GlobalBoundScope? s_globalScope;

    public static GlobalBoundScope GlobalScope
    {
        get
        {
            if (s_globalScope is null)
                Interlocked.CompareExchange(ref s_globalScope, new GlobalBoundScope(), null);
            return s_globalScope;
        }
    }

    protected Dictionary<string, Symbol>? Symbols { get; set; }

    public BoundScope Parent { get => parent ?? GlobalScope; }

    public bool Declare(Symbol symbol) => (Symbols ??= []).TryAdd(symbol.Name, symbol);

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
    internal GlobalBoundScope() : base()
    {
        Symbols = new Dictionary<string, Symbol>()
        {
            [PredefinedTypeNames.Any] = Any = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.AnyKeyword), PredefinedTypes.Any),
            [PredefinedTypeNames.Unknown] = Unknown = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.UnknownKeyword), PredefinedTypes.Unknown),
            [PredefinedTypeNames.Never] = Never = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.NeverKeyword), PredefinedTypes.Never),
            [PredefinedTypeNames.Unit] = Unit = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.UnitKeyword), PredefinedTypes.Unit),
            [PredefinedTypeNames.Type] = Type = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.TypeKeyword), PredefinedTypes.Type),
            [PredefinedTypeNames.Str] = Str = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.StrKeyword), PredefinedTypes.Str),
            [PredefinedTypeNames.Bool] = Bool = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.BoolKeyword), PredefinedTypes.Bool),
            [PredefinedTypeNames.I8] = I8 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I8Keyword), PredefinedTypes.I8),
            [PredefinedTypeNames.I16] = I16 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I16Keyword), PredefinedTypes.I16),
            [PredefinedTypeNames.I32] = I32 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I32Keyword), PredefinedTypes.I32),
            [PredefinedTypeNames.I64] = I64 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I64Keyword), PredefinedTypes.I64),
            [PredefinedTypeNames.I128] = I128 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I128Keyword), PredefinedTypes.I128),
            [PredefinedTypeNames.ISize] = ISize = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.ISizeKeyword), PredefinedTypes.ISize),
            [PredefinedTypeNames.U8] = U8 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U8Keyword), PredefinedTypes.U8),
            [PredefinedTypeNames.U16] = U16 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U16Keyword), PredefinedTypes.U16),
            [PredefinedTypeNames.U32] = U32 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U32Keyword), PredefinedTypes.U32),
            [PredefinedTypeNames.U64] = U64 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U64Keyword), PredefinedTypes.U64),
            [PredefinedTypeNames.U128] = U128 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U128Keyword), PredefinedTypes.U128),
            [PredefinedTypeNames.USize] = USize = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.USizeKeyword), PredefinedTypes.USize),
            [PredefinedTypeNames.F16] = F16 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F16Keyword), PredefinedTypes.F16),
            [PredefinedTypeNames.F32] = F32 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F32Keyword), PredefinedTypes.F32),
            [PredefinedTypeNames.F64] = F64 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F64Keyword), PredefinedTypes.F64),
            [PredefinedTypeNames.F80] = F80 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F80Keyword), PredefinedTypes.F80),
            [PredefinedTypeNames.F128] = F128 = new StructSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F128Keyword), PredefinedTypes.F128),
        };
    }

    public StructSymbol Any { get; }
    public StructSymbol Unknown { get; }
    public StructSymbol Never { get; }
    public StructSymbol Unit { get; }
    public StructSymbol Type { get; }
    public StructSymbol Str { get; }
    public StructSymbol Bool { get; }
    public StructSymbol I8 { get; }
    public StructSymbol I16 { get; }
    public StructSymbol I32 { get; }
    public StructSymbol I64 { get; }
    public StructSymbol I128 { get; }
    public StructSymbol ISize { get; }
    public StructSymbol U8 { get; }
    public StructSymbol U16 { get; }
    public StructSymbol U32 { get; }
    public StructSymbol U64 { get; }
    public StructSymbol U128 { get; }
    public StructSymbol USize { get; }
    public StructSymbol F16 { get; }
    public StructSymbol F32 { get; }
    public StructSymbol F64 { get; }
    public StructSymbol F80 { get; }
    public StructSymbol F128 { get; }
}
