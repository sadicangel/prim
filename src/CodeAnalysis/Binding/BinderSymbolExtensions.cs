using System.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal static class BinderSymbolExtensions
{
    extension(Symbol symbol)
    {
        public Result<BoundNode> BindDeclared()
        {
            if (symbol.IsPredefined)
            {
                // If we were to bind predefined symbols, we would need to ensure that the binder can handle them correctly,
                // which could introduce complexity and potential for errors. By treating them as already bound, we can simplify the binding process.
                Debug.Assert(symbol.ContainingModule.IsGlobal, "Predefined symbols should be contained in the global module.");
                return new Result<BoundNode>(new BoundPredefinedDeclaration(symbol), []);
            }

            var binder = symbol.GetBinder();
            var boundNode = binder.BindNode(symbol.Syntax);
            return new Result<BoundNode>(boundNode, [.. binder.GetDiagnostics()]);
        }

        public Binder GetBinder() => symbol.EnumerateContainingSymbolsFromGlobal()
            .Aggregate<Symbol, Binder>(null!, CreateBinder);

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
