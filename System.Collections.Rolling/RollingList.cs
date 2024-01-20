using System.Collections.Rolling.ValueBinders;
using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Rolling
{
    public interface IRollingList<T> : IList<T>, IRolling
    {

    }

    //[Experimental("RL0001")]
    public sealed class RollingList<T> : IRollingList<T>
    {
        internal readonly List<BufferSingle<T>> _core;

        internal int _version;

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

        public T this[int index]
        {
            get
            {
                return _core[index].Value;
            }
            set
            {
                _core[index] = Create(value);
            }
        }

        // offers support of capacity buffer.
        public RollingList(int capacity)
        {
            _core = new List<BufferSingle<T>>(capacity);

            CapacityBuffered = true;
            Capacity = capacity;
        }

        // offers support of period buffer.
        public RollingList(TimeSpan expiryPeriod)
        {
            _core = [];

            Expiration = (int)expiryPeriod.TotalMilliseconds;
            ExpiryBuffered = true;

            Capacity = int.MaxValue;
            CapacityBuffered = false;
        }

        // offers support of capacity & period buffer.
        public RollingList(TimeSpan expiryPeriod, int capacity)
        {
            _core = [];

            Expiration = (int)expiryPeriod.TotalMilliseconds;
            ExpiryBuffered = true;

            Capacity = capacity;
            CapacityBuffered = true;

        }

        // offers support of capacity buffer based on the length of the provided base range.
        public RollingList(T[] baseRange)
        {
            var range = new List<BufferSingle<T>>();

            Capacity = range.Capacity;
            CapacityBuffered = true;

            Expiration = -1;
            ExpiryBuffered = false;

            for (int i = 0; i < baseRange.Length; i++)
            {
                range.Add(Create(baseRange[i]));
            }

            _core = range;
        }

        BufferSingle<T> Create(T item)
        {
            if (ExpiryBuffered)
            {
                return new BufferSingle<T>(item, Expiration, () => Remove(item))
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
                    if (_core[i].Value == null)
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    if (_core[i].Value != null && _core[i]!.Equals(item)) return i;
                }
            }
            return -1;
        }

        // This functionality will be backwards, with value 0 becoming -1, rather than 99 becoming 100
        public void Insert(int index, T item)
        {
            _core.Insert(index, Create(item));

            if (_core.Count > Capacity)
            {
                // shift back the index
                _core[0].EarlyCancel();
            }
            _version++;
        }

        public void RemoveAt(int index)
        {
            _core.RemoveAt(index);
            _version++;
        }

        public void Add(T item)
        {
            _core.Add(Create(item));

            if (_core.Count > Capacity)
            {
                // shift back the index
                _core[0].EarlyCancel();
            }
            _version++;
        }

        public void AddRange(IEnumerable<T> items)
        {
            // needs change for version increment
            foreach (var item in items)
                Add(item);
        }

        public void Clear()
        {
            _core.Clear();
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_core[i].Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // this operation is expensive, is there another way?
            _core.Select(x => x.Value)
                .ToArray()
                .CopyTo(array, arrayIndex);
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
            return new BufferEnumerator<T>(this);
        }

        // ???
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
