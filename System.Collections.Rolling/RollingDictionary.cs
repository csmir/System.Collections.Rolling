namespace System.Collections.Rolling
{
    public interface IRollingDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IRolling
    {
    }

    public sealed class RollingDictionary<TKey, TValue> : IRollingDictionary<TKey, TValue>
    {
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
    }

    public static class RollingDictionary
    {
        public static RollingDictionaryBuilder<TKey, TValue> CreateBuilder<TKey, TValue>()
        {

        }
    }
}
