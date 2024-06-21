using System.Diagnostics;

namespace CodeAnalysis;

internal class CollectionDebugView<T>
{
    private readonly ICollection<T> _collection;

    public CollectionDebugView(ICollection<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        _collection = collection;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items
    {
        get
        {
            var array = new T[_collection.Count];
            _collection.CopyTo(array, 0);
            return array;
        }
    }
}
