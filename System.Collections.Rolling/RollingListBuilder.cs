using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Rolling
{
    public sealed class RollingListBuilder<T>
    {
        private int _cap = 0;
        private TimeSpan _period = TimeSpan.Zero;

        public RollingListBuilder<T> WithCapacity(int capacity)
        {
            _cap = capacity;
        }

        public RollingListBuilder<T> WithExpiry(TimeSpan expiration)
        {
            _period = expiration;
        }
    }
}
