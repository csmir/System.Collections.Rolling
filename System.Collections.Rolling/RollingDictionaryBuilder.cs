using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Rolling
{
    public sealed class RollingDictionaryBuilder<TKey, TValue>
    {
        private int _cap = 0;
        private TimeSpan _period = TimeSpan.Zero;

        public RollingDictionaryBuilder<TKey, TValue> WithCapacity(int capacity)
        {
            _cap = capacity;
        }

        public RollingDictionaryBuilder<TKey, TValue> WithExpiry(TimeSpan expiration)
        {
            _period = expiration;
        }
    }
}
