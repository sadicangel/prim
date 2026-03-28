using System.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal static class BinderSymbolExtensions
{
    extension(Symbol symbol)
    {
        public Result<BoundNode> Bind()
        {
            var binder = symbol.EnumerateContainingSymbolsFromGlobal()
                .Aggregate<Symbol, Binder>(null!, CreateBinder);

            var boundNode = binder.BindNode(symbol.Syntax);
            return new Result<BoundNode>(boundNode, [.. binder.GetDiagnostics()]);
        }

        private IEnumerable<Symbol> EnumerateContainingSymbolsFromGlobal()
        {
            var stack = new Stack<Symbol>();
            var current = symbol;
            do
            {
                current = current.ContainingSymbol;
                stack.Push(current);
            } while (current is not ModuleSymbol { IsGlobal: true });

            if (stack.Count == 0) throw new UnreachableException("Unexpected Binder state");

            while (stack.Count > 0)
            {
                yield return stack.Pop();
            }
        }
    }

    private static Binder CreateBinder(Binder? parent, Symbol symbol) => symbol switch
    {
        ModuleSymbol moduleSymbol => new ModuleBinder(moduleSymbol, parent),
        TypeSymbol typeSymbol => new TypeBinder(typeSymbol, parent),
        _ => throw new UnreachableException($"Unexpected {nameof(Symbol)} '{symbol?.GetType()}' as containing symbol"),
    };
}
