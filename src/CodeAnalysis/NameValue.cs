using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public readonly struct NameValue
{
    private readonly object? _names;

    public NameValue(string name)
    {
        _names = name;
    }

    public NameValue(string[] name)
    {
        _names = name;
    }

    public int Count
    {
        get
        {
            if (_names is null)
            {
                return 0;
            }
            else if (_names is string)
            {
                return 1;
            }
            else
            {
                // Not string, not null, can only be string[]
                return Unsafe.As<string?[]>(_names).Length;
            }
        }
    }

    public override string ToString()
    {
        return _names switch
        {
            string s => s,
            string[] a => a.Length switch
            {
                0 => string.Empty,
                1 => a[0],
                _ => string.Join(SyntaxFacts.GetText(SyntaxKind.ColonColonToken), a),
            },
            _ => string.Empty,
        };
    }

    public string[] ToArray()
    {
        return _names switch
        {
            string s => [s],
            string[] a => a,
            _ => [],
        };
    }

    public static implicit operator NameValue(string name) => new(name);
    public static implicit operator string(NameValue names) => names.ToString();

    public static implicit operator NameValue(string[] names) => new(names);
    public static implicit operator string[](NameValue names) => names.ToArray();
}
