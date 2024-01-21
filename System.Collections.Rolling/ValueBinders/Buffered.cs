using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Rolling
{
    [DebuggerDisplay("Value = {Value}")]
    internal record class Buffered<T>
    {
        public T Value { get; }

        private readonly int _period;
        private readonly Action _queuedRemoval;
        private CancellationTokenSource _cancelSource;
        private bool _reset;

        public Buffered(T value, int period, Action queuedRemoval)
        {
            Value = value;

            _period = period;
            _queuedRemoval = queuedRemoval;
            _cancelSource = new();
        }

        public Buffered(T value)
        {
            Value = value;

            _period = -1;
            _queuedRemoval = null!;
            _cancelSource = null!;
        }

        public Buffered<T> Start()
        {
            if (_period >= 0)
            {
                Run(_cancelSource.Token);
            }

            return this;
        }

        public void Run(CancellationToken token)
        {
            _reset = false;

            Task.Delay(_period, token);

            if (_reset)
                return;

            lock (this) // will Value be destroyed when GC cleans up the container?
                        // if not, this is irrelevant.
                        // should return this type to `struct`, perhaps even `readonly struct`
            {
                _queuedRemoval();
            }
        }

        public void Reset()
        {
            _reset = true;
            _cancelSource.Cancel();

            _cancelSource = new();
            Run(_cancelSource.Token);
        }

        public void EarlyCancel()
        {
            // Is this necessary?
            _cancelSource.Cancel();

            _queuedRemoval();
        }
    }
}
