using CodeAnalysis.Text;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Repl;

internal abstract class ReplBase
{
    private readonly List<string> _history = new();
    private readonly string _commandPrefix;
    private readonly SortedSet<Command> _commands;
    private int _historyIndex;
    private bool _done;

    protected ReplBase(string commandPrefix)
    {
        if (String.IsNullOrWhiteSpace(commandPrefix))
            throw new ArgumentException($"'{nameof(commandPrefix)}' cannot be null or whitespace.", nameof(commandPrefix));

        _commandPrefix = commandPrefix;

        _commands = InitCommands();
    }

    private SortedSet<Command> InitCommands()
    {
        var commands = new SortedSet<Command>();

        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        var methods = typeof(ReplBase).GetMethods(flags).Concat(GetType().GetMethods(flags));
        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<CommandAttribute>();
            if (attribute is null)
                continue;

            if (!commands.Add(new Command(attribute, method)))
                throw new InvalidOperationException($"Command '{attribute.Name}' already defined");
        }

        return commands;
    }

    public void Run()
    {
        while (true)
        {
            var input = ReadInput();

            if (String.IsNullOrEmpty(input))
                return;

            if (!input.Contains(Environment.NewLine) && input.StartsWith(_commandPrefix))
            {
                EvaluateCommand(input.AsSpan(_commandPrefix.Length));
            }
            else
            {
                Evaluate(input);
            }

            _history.Add(input);
            _historyIndex = 0;
        }
    }

    private string? ReadInput()
    {
        _done = false;
        var document = new ObservableCollection<string> { "" };
        var view = new InputView(RenderLine, document);

        while (!_done)
        {
            var key = Console.ReadKey(intercept: true);
            HandleKey(key, document, view);
        }

        view.CurrentLineIndex = document.Count - 1;
        view.CurrentCharacter = document[view.CurrentLineIndex].Length;

        Console.WriteLine();

        return String.Join(Environment.NewLine, document);
    }

    private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, InputView view)
    {
        switch (key.Modifiers)
        {
            case ConsoleModifiers.Control:
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleControlEnter(document, view);
                        break;
                }
                return;

            case ConsoleModifiers.Shift:
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleShiftEnter(document, view);
                        break;

                    case ConsoleKey when key.KeyChar >= ' ':
                        HandleTyping(document, view, key.KeyChar.ToString());
                        break;
                }
                return;

            case ConsoleModifiers.Control | ConsoleModifiers.Alt:
                switch (key.Key)
                {
                    case ConsoleKey when key.KeyChar >= ' ':
                        HandleTyping(document, view, key.KeyChar.ToString());
                        break;
                }
                return;

            case default(ConsoleModifiers):
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        HandleEscape(document, view);
                        break;
                    case ConsoleKey.Enter:
                        HandleEnter(document, view);
                        break;
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(document, view);
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(document, view);
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow(document, view);
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow(document, view);
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackspace(document, view);
                        break;
                    case ConsoleKey.Delete:
                        HandleDelete(document, view);
                        break;
                    case ConsoleKey.Home:
                        HandleHome(document, view);
                        break;
                    case ConsoleKey.End:
                        HandleEnd(document, view);
                        break;
                    case ConsoleKey.Tab:
                        HandleTab(document, view);
                        break;
                    case ConsoleKey.PageUp:
                        HandlePageUp(document, view);
                        break;
                    case ConsoleKey.PageDown:
                        HandlePageDown(document, view);
                        break;
                    case ConsoleKey when key.KeyChar >= ' ':
                        HandleTyping(document, view, key.KeyChar.ToString());
                        break;
                    default:
                        break;
                }
                return;
        }
    }

    private void HandleEscape(ObservableCollection<string> document, InputView view)
    {
        document.Clear();
        document.Add(String.Empty);
        view.CurrentCharacter = 0;
        view.CurrentLineIndex = 0;
    }

    private void HandleEnter(ObservableCollection<string> document, InputView view)
    {
        var input = String.Join(Environment.NewLine, document);
        if (input.StartsWith("\\") || IsCompleteInput(input))
        {
            _done = true;
            return;
        }

        InsertLine(document, view);
    }

    private void HandleShiftEnter(ObservableCollection<string> document, InputView view)
    {
        InsertLine(document, view);
    }

    private void HandleControlEnter(ObservableCollection<string> document, InputView view)
    {
        _done = true;
        return;
    }

    private static void InsertLine(ObservableCollection<string> document, InputView view)
    {
        var remainder = document[view.CurrentLineIndex][view.CurrentCharacter..];
        document[view.CurrentLineIndex] = document[view.CurrentLineIndex][..view.CurrentCharacter];

        var lineIndex = view.CurrentLineIndex + 1;
        document.Insert(lineIndex, remainder);
        view.CurrentCharacter = 0;
        view.CurrentLineIndex = lineIndex;
    }

    private void HandleLeftArrow(ObservableCollection<string> document, InputView view)
    {
        if (view.CurrentCharacter > 0)
            view.CurrentCharacter--;
    }

    private void HandleRightArrow(ObservableCollection<string> document, InputView view)
    {
        if (view.CurrentCharacter < document[view.CurrentLineIndex].Length)
            view.CurrentCharacter++;
    }

    private void HandleUpArrow(ObservableCollection<string> document, InputView view)
    {
        if (view.CurrentLineIndex > 0)
            view.CurrentLineIndex--;
    }

    private void HandleDownArrow(ObservableCollection<string> document, InputView view)
    {
        if (view.CurrentLineIndex < document.Count - 1)
            view.CurrentLineIndex++;
    }

    private void HandleHome(ObservableCollection<string> document, InputView view)
    {
        view.CurrentCharacter = 0;
    }

    private void HandleEnd(ObservableCollection<string> document, InputView view)
    {
        view.CurrentCharacter = document[view.CurrentLineIndex].Length;
    }

    private void HandleTab(ObservableCollection<string> document, InputView view)
    {
        const int TabWidth = 4;
        var start = view.CurrentCharacter;
        var remainingSpaces = TabWidth - start % TabWidth;
        var line = document[view.CurrentLineIndex];
        document[view.CurrentLineIndex] = line.Insert(start, new string(' ', remainingSpaces));
        view.CurrentCharacter += remainingSpaces;
    }

    private void HandlePageUp(ObservableCollection<string> document, InputView view)
    {
        _historyIndex--;
        if (_historyIndex < 0)
            _historyIndex = _history.Count - 1;
        UpdateDocumentFromHistory(document, view);
    }

    private void HandlePageDown(ObservableCollection<string> document, InputView view)
    {
        _historyIndex++;
        if (_historyIndex > _history.Count - 1)
            _historyIndex = 0;
        UpdateDocumentFromHistory(document, view);
    }

    private void UpdateDocumentFromHistory(ObservableCollection<string> document, InputView view)
    {
        if (_history.Count == 0)
            return;

        document.Clear();

        var historyItem = _history[_historyIndex];
        var lines = historyItem.Split(Environment.NewLine);
        foreach (var line in lines)
            document.Add(line);
        view.CurrentLineIndex = document.Count - 1;
        view.CurrentCharacter = document[view.CurrentLineIndex].Length;
    }

    private void HandleDelete(ObservableCollection<string> document, InputView view)
    {
        var lineIndex = view.CurrentLineIndex;
        var line = document[lineIndex];
        var start = view.CurrentCharacter;
        if (start >= line.Length)
        {
            if (view.CurrentLineIndex == document.Count - 1)
                return;

            var nextLine = document[view.CurrentLineIndex + 1];
            document[view.CurrentLineIndex] += nextLine;
            document.RemoveAt(view.CurrentLineIndex + 1);
        }
        else
        {
            var before = line[..start];
            var after = line[(start + 1)..];
            document[lineIndex] = before + after;
        }
    }

    private void HandleBackspace(ObservableCollection<string> document, InputView view)
    {
        var start = view.CurrentCharacter;
        if (start == 0)
        {
            if (view.CurrentLineIndex == 0)
                return;

            var currentLine = document[view.CurrentLineIndex];
            var previousLine = document[view.CurrentLineIndex - 1];
            document.RemoveAt(view.CurrentLineIndex);
            view.CurrentLineIndex--;
            document[view.CurrentLineIndex] = previousLine + currentLine;
            view.CurrentCharacter = previousLine.Length;
        }
        else
        {
            var lineIndex = view.CurrentLineIndex;
            var line = document[lineIndex];
            var before = line[..(start - 1)];
            var after = line[start..];
            document[lineIndex] = before + after;
            view.CurrentCharacter--;
        }
    }

    private void HandleTyping(ObservableCollection<string> document, InputView view, string text)
    {
        var lineIndex = view.CurrentLineIndex;
        var start = view.CurrentCharacter;
        document[lineIndex] = document[lineIndex].Insert(start, text);
        view.CurrentCharacter += text.Length;
    }

    protected void ClearHistory() => _history.Clear();
    protected virtual object? RenderLine(IReadOnlyList<string> lines, int lineIndex, object? state)
    {
        Console.Write(lines[lineIndex]);
        return state;
    }

    protected abstract bool IsCompleteInput(string text);
    protected abstract void Evaluate(string input);

    private bool EvaluateCommand(ReadOnlySpan<char> input)
    {
        var index = input.IndexOf(' ');
        var cmd = index >= 0 ? input[..index] : input;

        foreach (var command in _commands)
        {
            if (command.Name.AsSpan().SequenceEqual(cmd))
            {
                var args = Array.Empty<string>();
                if (index >= 0)
                {
                    args = new string(input[(index + 1)..]).Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < args.Length; ++i)
                    {
                        if (args[i] is ['\"', .., '\"'])
                            args[i] = args[i][1..^1];
                    }
                }

                if (args.Length != command.Parameters.Length)
                {
                    Console.Out.WriteLineColored($"Command '{cmd}' requires {command.Parameters.Length} arguments but was given {args.Length}", ConsoleColor.DarkRed);
                    Console.Out.WriteColored($"Usage: ", ConsoleColor.DarkGray);
                    Console.Write(command.DisplayName);
                    Console.WriteLine();
                    return true;
                }

                command.Method.Invoke(this, args);
                return command.Name is not "exit";
            }
        }
        Console.Out.WriteLineColored($"[unknown command '{cmd}']", ConsoleColor.DarkRed);
        HelpCommand();
        return true;
    }

    [Command("help", Description = "Show help")]
    private void HelpCommand()
    {
        Console.WriteLine("Available commands:");
        var maxDisplayNameLength = _commands.Max(cmd => cmd.DisplayName.Length);
        Span<char> displayName = stackalloc char[maxDisplayNameLength];
        foreach (var command in _commands)
        {
            displayName.Fill(' ');
            command.DisplayName.CopyTo(displayName);
            Console.Out.WriteColored(_commandPrefix, ConsoleColor.DarkYellow);
            Console.Out.WriteColored(displayName, ConsoleColor.DarkYellow);
            Console.Out.Write("    ");
            Console.Out.Write(command.Description);
            Console.WriteLine();
        }
    }

    [Command("cls", Description = "Clear window")]
    private void ClsCommand() => Console.Clear();

    [Command("exit", Description = "Exit REPL")]
    private void ExitCommand() { }
}
