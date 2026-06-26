using System.Collections.Immutable;

namespace CodeAnalysis.Syntax;

internal record class SyntaxTokenStream(ImmutableArray<SyntaxToken> Tokens)
{
    private int _offset = 0;

    public bool IsAtEnd => Peek().Kind == SyntaxKind.EofToken;

    public SyntaxToken Peek(int offset = 0) => Tokens[int.Min(_offset + offset, Tokens.Length - 1)];

    public SyntaxKind PeekKind(int offset = 0) => Peek(offset).Kind;

    public SyntaxToken Next()
    {
        var current = Peek();
        if (_offset < Tokens.Length) _offset += 1;
        return current;
    }

    public void Skip(int count = 1) => _offset = Math.Min(_offset + count, Tokens.Length - 1);

    public SavePoint Checkpoint() => new(this);

    public readonly ref struct SavePoint : IDisposable
    {
        private readonly SyntaxTokenStream _stream;
        private readonly int _offset;

        internal SavePoint(SyntaxTokenStream stream)
        {
            _stream = stream;
            _offset = stream._offset;
        }


        public void Dispose() => _stream._offset = _offset;
    }
}

