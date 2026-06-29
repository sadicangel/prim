using System.Collections;
using System.Text;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public readonly record struct NameString(string FullName) : IEnumerable<string>, IEquatable<string>
{
    public const string GlobalName = "<global>";

    public string Name { get; } = GetName(FullName);

    public bool IsEmpty => string.IsNullOrEmpty(FullName);

    public bool IsSimple => !FullName.Contains('.');

    public string FullyQualifiedName => $"{GlobalName}.{FullName}";

    public NameString(IEnumerable<string> parts) : this(string.Empty)
    {
        using var enumerator = parts.GetEnumerator();
        if (!enumerator.MoveNext()) return;
        Name = enumerator.Current;
        var fullName = new StringBuilder(Name);

        while (enumerator.MoveNext() && enumerator.Current != GlobalName)
        {
            Name = enumerator.Current;
            fullName.Append(SyntaxFacts.NameSeparator).Append(Name);
        }

        FullName = fullName.ToString();
    }

    public NameString(string @namespace, string name) : this(string.Empty)
    {
        FullName = $"{@namespace}.{name}";
        Name = name;
    }

    private static string GetName(string fullName)
    {
        var name = string.Empty;
        using var enumerator = GetEnumerator(fullName);
        while (enumerator.MoveNext()) name = enumerator.Current;
        return name;
    }

    private static IEnumerator<string> GetEnumerator(string fullName)
    {
        if (string.IsNullOrEmpty(fullName)) yield break;

        var start = 0;
        foreach (var (index, @char) in fullName.Index())
        {
            if (@char != SyntaxFacts.NameSeparator) continue;

            yield return fullName[start..index];
            start = index + 1;
        }

        yield return fullName[start..];
    }

    public IEnumerator<string> GetEnumerator() => GetEnumerator(FullName);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(string? other) => FullName == other;

    public override string ToString() => FullName;

    public static NameString operator +(NameString left, string right) => new(left.FullName, right);

    public static implicit operator NameString(string fullName) => new(fullName);

    public static bool operator ==(NameString left, string right) => left.Equals(right);
    public static bool operator !=(NameString left, string right) => !(left == right);

    public static bool operator ==(string left, NameString right) => right.Equals(left);
    public static bool operator !=(string left, NameString right) => !(right == left);
}
