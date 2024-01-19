using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Rolling
{
    public interface IRolling
    {
        public bool CapacityBuffered { get; }

        public int Capacity { get; }

        public bool ExpiryBuffered { get; }

        public int Expiration { get; }
    }
}
