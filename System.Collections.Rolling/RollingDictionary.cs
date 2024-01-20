using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Rolling
{
    public interface IRollingDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IRolling
    {
    }

    //[Experimental("RD0001")]
    public sealed class RollingDictionary<TKey, TValue> : IRollingDictionary<TKey, TValue>
    {
        private readonly Dictionary<BufferSingle<TKey>, TValue> _core;

        public ICollection<TKey> Keys
        {
            get
            {
                return _core.Keys.Select(x => x.Value).ToArray();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return _core.Values;
            }
        }

        public int Count
        {
            get
            {
                return _core.Count;
            }
        }

        public bool IsReadOnly { get; } = false;

        public bool CapacityBuffered { get; }

        public int Capacity { get; }

        public bool ExpiryBuffered { get; }

        public int Expiration { get; }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                    return value;
                throw new KeyNotFoundException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        // offers support of capacity buffer.
        public RollingDictionary(int capacity)
        {

        }

        // offers support of period buffer.
        public RollingDictionary(TimeSpan expiryPeriod)
        {

        }

        // offers support of capacity & period buffer.
        public RollingDictionary(TimeSpan expiryPeriod, int capacity)
        {

        }

        // offers support of capacity buffer based on the length of the provided base range.
        public RollingDictionary(IEnumerable<KeyValuePair<TKey, TValue>> baseRange)
        {

        }

        KeyValuePair<BufferSingle<TKey>, TValue> Create(KeyValuePair<TKey, TValue> item)
        {
            if (ExpiryBuffered)
            {
                return new(new BufferSingle<TKey>(item.Key, Expiration, () => Remove(item.Key)).Start(), item.Value);
            }
            else
                return new(new(item.Key), item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
