namespace Repl;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class CommandAttribute : Attribute
{
    public CommandAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public string Description { get; init; } = String.Empty;
}
