using System.Diagnostics;

namespace System.Collections.Rolling
{
    public interface IRollingList<T> : IList<T>, IRolling
    {

    }

    //[Experimental("RL0001")]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]                       // serialization should not keep contacted values as buffered, and should convert them to an uncontained T[] instead.
                                         // would ISerializable be a better option?
    public sealed class RollingList<T> : IRollingList<T>
    {
        // as specified in List<>.
        private const int DefaultCapacity = 4;
        
        internal Buffered<T>[] _items; // binary serialization(?) -> this might not work, unless if Buffered<> is serializable?
        internal int _size;            // binary serialization
        internal int _version;         // ...
        internal int _expiry;          // ...

#pragma warning disable CA1825 // avoid the extra generic instantiation for Array.Empty<T>()
        private static readonly Buffered<T>[] s_emptyArray = new Buffered<T>[0];
#pragma warning restore CA1825

        public int Count => _items.Length;

        public bool CapacityBuffered { get; private set; }

        public int Capacity
        {
            get
            {
                return _size;
            }
            set
            {
                // changing the size of the list through the setter should automatically bind CapacityBuffered to true, as it then is hard-set.

                if (value < _size)
                {
                    // THROW range too small
                }

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        Buffered<T>[] newItems = new Buffered<T>[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, newItems, _size);
                        }
                        _items = newItems;
                        CapacityBuffered = true;
                    }
                    else
                    {
                        _items = s_emptyArray; // should we support empty rolling arrays?
                                               // if so, should this trigger CapacityBuffered to false? -> might result in unintended behavior on user-side.
                    }
                }
            }
        }

        public bool ExpiryBuffered { get; private set; }

        public int Expiration
        {
            get
            {
                // will always return -1 if set to 0 or anything below.
                return _expiry;
            }
            set
            {
                // changing the expiration from -1 to anything else should automatically bind ExpiryBuffered as true, otherwise default to false.
                if (value <= 0)
                {
                    ExpiryBuffered = false;
                    _expiry = -1;
                }
                else
                {
                    ExpiryBuffered = true;
                    _expiry = value;
                }
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                return _items[index].Value;
            }
            set
            {
                _items[index] = Create(value);
            }
        }

        // offers support of capacity buffer.
        public RollingList(int capacity)
        {
            _items = new Buffered<T>[capacity];

            CapacityBuffered = true;
            _size = capacity;
        }

        // offers support of period buffer.
        public RollingList(TimeSpan expiryPeriod)
        {
            _items = [];

            Expiration = (int)expiryPeriod.TotalMilliseconds;
            ExpiryBuffered = true;

            _size = DefaultCapacity;
            CapacityBuffered = false;
        }

        // offers support of capacity & period buffer.
        public RollingList(TimeSpan expiryPeriod, int capacity)
        {
            _items = [];

            Expiration = (int)expiryPeriod.TotalMilliseconds;
            ExpiryBuffered = true;

            _size = capacity;
            CapacityBuffered = true;

        }

        // offers support of capacity buffer based on the length of the provided base range.
        public RollingList(T[] baseRange)
        {
            var range = new Buffered<T>[baseRange.Length];

            _size = baseRange.Length;
            CapacityBuffered = true;

            Expiration = -1;
            ExpiryBuffered = false;

            for (int i = 0; i < baseRange.Length; i++)
            {
                range[i] = Create(baseRange[i]);
            }

            _items = range;
        }

        Buffered<T> Create(T item)
        {
            if (ExpiryBuffered)
            {
                return new Buffered<T>(item, Expiration, () => Remove(item))
                    .Start();
            }
            else
                return new(item);
        }

        public int IndexOf(T item)
        {
            if (item == null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (_items[i].Value == null)
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    if (_items[i].Value != null && _items[i]!.Equals(item)) return i;
                }
            }
            return -1;
        }

        // This functionality will be backwards, with value 0 becoming -1, rather than 99 becoming 100
        public void Insert(int index, T item)
        {
            //_items.Insert(index, Create(item));

            //if (_items.Length > Capacity)
            //{
            //    // shift back the index
            //    _items[0].EarlyCancel();
            //}
            //_version++;
        }

        public void RemoveAt(int index)
        {
            //_items.RemoveAt(index);
            //_version++;
        }

        public void Add(T item)
        {
            //_items.Add(Create(item));

            //if (_items.Count > Capacity)
            //{
            //    // shift back the index
            //    _items[0].EarlyCancel();
            //}
            //_version++;
        }

        public void AddRange(IEnumerable<T> items)
        {
            // needs change for version increment
            foreach (var item in items)
                Add(item);
        }

        public void Clear()
        {
            //_items.Clear();
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_items[i].Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ContainedArray.UncontainedClone(_items).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var i = IndexOf(item);

            if (i >= 0)
            {
                RemoveAt(i);
                return true;
            }
            return false;
        }

        // ???
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        // ???
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            internal static IEnumerator<T>? s_emptyEnumerator;

            private readonly RollingList<T> _list;
            private int _index;
            private readonly int _version;
            private T? _current;

            internal Enumerator(RollingList<T> list)
            {
                _list = list;
                _index = 0;
                _version = list._version;
                _current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                RollingList<T> localList = _list;

                if (_version == localList._version && ((uint)_index < (uint)localList.Count))
                {
                    _current = localList._items[_index].Value;
                    _index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (_version != _list._version)
                {
                    // THROW
                }

                _index = _list.Count + 1;
                _current = default;
                return false;
            }

            public T Current => _current!;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _list.Count + 1)
                    {
                        // THROW 
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _list._version)
                {
                    // THROW failedvers
                }

                _index = 0;
                _current = default;
            }
        }
    }
}
