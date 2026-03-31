using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binding;

internal sealed class LambdaBinder(LambdaTypeSymbol lambdaTypeType, Binder parent) : Binder(parent)
{
    private Dictionary<string, VariableSymbol>? _parameters;
    private HashSet<VariableSymbol>? _captures;

    public LambdaTypeSymbol LambdaType => lambdaTypeType;

    public IEnumerable<VariableSymbol> Parameters => _parameters?.Values.AsEnumerable() ?? [];

    /// <inheritdoc />
    public override ModuleSymbol Module => Parent!.Module;

    /// <inheritdoc />
    public override bool TryDeclare(Symbol symbol) => throw new UnreachableException($"Unexpected declaration in {nameof(LambdaBinder)}");

    public override bool TryLookup<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol)
    {
        if (TryLookupInCurrentScope(name, out symbol)) return true;
        if (!Parent!.TryLookup(name, out symbol)) return false;
        if (symbol is VariableSymbol variable) (_captures ??= []).Add(variable);
        return true;
    }

    /// <inheritdoc />
    protected override bool TryLookupInCurrentScope<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol)
    {
        symbol = null;
        if (typeof(TSymbol) != typeof(VariableSymbol)) return false;
        if (_parameters?.TryGetValue(name, out var variable) is not true) return false;
        symbol = variable as TSymbol;
        return symbol is not null;
    }

    public void BindParameters(LambdaExpressionSyntax syntax)
    {
        if (_parameters is not null)
        {
            throw new UnreachableException($"Unexpected redeclaration of parameters in {nameof(LambdaBinder)}");
        }

        var parameterTypes = LambdaType.Parameters;
        var parameterNames = syntax.Parameters.ToImmutableArray();
        if (parameterNames.Length != parameterTypes.Length)
        {
            var sourceSpan = SourceSpan.Union(syntax.ParenthesisOpenToken.SourceSpan, syntax.ParenthesisCloseToken.SourceSpan);
            this.ReportInvalidParameterCount(sourceSpan, parameterTypes.Length, parameterNames.Length);

            var difference = parameterNames.Length - parameterTypes.Length;
            if (difference > 0)
                parameterTypes = parameterTypes.AddRange(Enumerable.Repeat(Module.Never, difference));
        }

        _parameters = new Dictionary<string, VariableSymbol>(parameterNames.Length);
        foreach (var (parameterName, parameterType) in parameterNames.Zip(parameterTypes))
        {
            var parameter = new VariableSymbol(parameterName, parameterName.FullName, parameterType, Module, Modifiers.ReadOnly);
            if (!_parameters.TryAdd(parameter.Name, parameter))
            {
                this.ReportSymbolRedeclaration(parameterName.SourceSpan, parameter.Name);
            }
        }
    }
}
