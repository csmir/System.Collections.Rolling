using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Rolling
{
    internal class BufferBoundValue<T>
    {
        public T Value { get; }

        private readonly int _period;
        private readonly Action _queuedRemoval;
        private CancellationTokenSource _cancelSource;
        private bool _reset;

        public BufferBoundValue(T value, int period, Action queuedRemoval)
        {
            Value = value;

            _period = period;
            _queuedRemoval = queuedRemoval;
            _cancelSource = new();
        }

        public BufferBoundValue(T value)
        {
            Value = value;

            _period = -1;
            _queuedRemoval = null!;
            _cancelSource = null!;
        }

        public BufferBoundValue<T> Start()
        {
            if (_period >= 0)
                Run(_cancelSource.Token);

            return this;
        }

        public async void Run(CancellationToken token)
        {
            _reset = false;
            await Task.Delay(_period, token);

            if (_reset)
                return;

            _queuedRemoval();
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
