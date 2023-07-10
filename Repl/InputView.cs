using CodeAnalysis.Text;
using System.Collections.ObjectModel;

namespace Repl;

public sealed record class InputView
{
    private readonly Action<string> _lineRenderer;
    private readonly ObservableCollection<string> _document;
    private readonly int _cursorTop;
    private int _renderedLineCount;
    private int _currentLineIndex;
    private int _currentCharacter;

    public InputView(Action<string> lineRenderer, ObservableCollection<string> document)
    {
        _lineRenderer = lineRenderer;
        _document = document;
        _document.CollectionChanged += Document_CollectionChanged;
        _cursorTop = Console.CursorTop;
        Render();
    }

    public int CurrentLineIndex
    {
        get => _currentLineIndex;
        set
        {
            if (_currentLineIndex != value)
            {
                _currentLineIndex = value;
                _currentCharacter = Math.Min(_currentCharacter, _document[_currentLineIndex].Length);
                UpdateCursorPosition();
            }
        }
    }
    public int CurrentCharacter
    {
        get => _currentCharacter;
        set
        {
            if (_currentCharacter != value)
            {
                _currentCharacter = value;
                UpdateCursorPosition();
            }
        }
    }

    private void Document_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Render();
    }

    private void Render()
    {
        Console.CursorVisible = false;
        var lineCount = 0;
        var isFirst = true;
        foreach (var line in _document)
        {
            Console.SetCursorPosition(0, _cursorTop + lineCount);
            Console.Out.WriteColored(isFirst ? "» " : "· ", ConsoleColor.Green);
            if (isFirst) isFirst = false;

            _lineRenderer.Invoke(line);
            Console.WriteLine(new string(' ', Console.WindowWidth - line.Length));

            lineCount++;
        }

        var blankLinesCount = _renderedLineCount - lineCount;
        if (blankLinesCount > 0)
        {
            var blankLine = new string(' ', Console.WindowWidth);
            for (var i = 0; i < blankLinesCount; ++i)
            {
                Console.SetCursorPosition(0, _cursorTop + lineCount + i);
                Console.WriteLine(blankLine);
            }
        }

        _renderedLineCount = lineCount;
        Console.CursorVisible = true;
        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        Console.CursorTop = _cursorTop + _currentLineIndex;
        Console.CursorLeft = 2 + _currentCharacter;
    }
}
