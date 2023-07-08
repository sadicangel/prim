using System.Collections.Immutable;
using System.Reflection;

namespace Repl;

internal sealed record class Command(string Name, string Description, MethodInfo Method) : IComparable<Command>
{
    public Command(CommandAttribute attribute, MethodInfo method) : this(attribute.Name, attribute.Description ?? String.Empty, method) { }

    public ImmutableArray<ParameterInfo> Parameters { get; } = Method.GetParameters().ToImmutableArray();

    public string DisplayName { get; } = GetDisplayName(Name, Method);

    private static string GetDisplayName(string name, MethodInfo method)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == 0)
            return name;

        return $"{name} {String.Join(" ", parameters.Select(p => $"<{p.Name}>"))}";
    }

    public bool Equals(Command? other) => other is not null && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public int CompareTo(Command? other) => other is null ? 1 : Name.CompareTo(other.Name);
}