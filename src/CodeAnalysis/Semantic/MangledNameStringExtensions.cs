namespace CodeAnalysis.Semantic;

internal static class MangledNameStringExtensions
{
    private static NameString Mangle<T>(string prefix, Func<T, string> toString, params ReadOnlySpan<T> objects)
    {
        if (objects.IsEmpty) return prefix;
        return $"{prefix}<{string.Join(", ", objects.ToArray().Select(toString))}>";
    }

    extension(OperatorKind operatorKind)
    {
        public NameString GetName(params ReadOnlySpan<TypeSymbol> operandTypes) =>
            Mangle(operatorKind.ToString(), type => type.Name.ToString(), operandTypes);
    }
}