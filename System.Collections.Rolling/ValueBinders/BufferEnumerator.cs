using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Rolling.ValueBinders
{
    public struct BufferEnumerator<T> : IEnumerator<T>, IEnumerator
    {
        internal static IEnumerator<T>? s_emptyEnumerator;

        private readonly RollingList<T> _list;
        private int _index;
        private readonly int _version;
        private T? _current;

        internal BufferEnumerator(RollingList<T> list)
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
                _current = localList._core[_index].Value;
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
