using System.Collections.ObjectModel;

namespace Repl;

internal abstract class ReplBase
{
    private readonly List<string> _history = new();
    private int _historyIndex;
    private bool _done;
    public void Run()
    {
        while (true)
        {
            var input = ReadInput();

            if (String.IsNullOrEmpty(input))
                return;

            if (!input.Contains(Environment.NewLine) && input.StartsWith("\\"))
            {
                EvaluateCommand(input);
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
        document[view.CurrentLineIndex] = String.Empty;
        view.CurrentCharacter = 0;
    }

    private void HandleEnter(ObservableCollection<string> document, InputView view)
    {
        var input = String.Join(Environment.NewLine, document);
        if (input.StartsWith("\\") || IsCompleteInput(input.AsMemory()))
        {
            _done = true;
            return;
        }

        InsertLine(document, view);
    }

    private void HandleControlEnter(ObservableCollection<string> document, InputView view)
    {
        InsertLine(document, view);
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
            return;

        var before = line[..start];
        var after = line[(start + 1)..];
        document[lineIndex] = before + after;
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

    protected virtual void RenderLine(string line) => Console.Write(line);

    protected abstract bool IsCompleteInput(ReadOnlyMemory<char> text);
    protected abstract bool EvaluateCommand(ReadOnlySpan<char> input);
    protected abstract void Evaluate(string input);
}