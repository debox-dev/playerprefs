using System;

namespace DeBox.PlayerPrefs
{
    public class QueueFullException : Exception
    {
    }

    public class QueueEmptyException : Exception
    {
    }

    /// <summary>
    /// A PlayerPrefs stored queue
    /// </summary>
    /// <typeparam name="T">The type of PlayerPrefs store to use</typeparam>
    /// <typeparam name="K">The type of object managed by that PlayerPrefs store</typeparam>
    public class PlayerPrefsQueue<T, K> where T : SimplePlayerPrefsValue<K>, new()
    {
        private const int INSERT_INDEX_FULL = -1;

        public readonly int Length;
        private readonly string _keyPrefix;
        private PlayerPrefsInt _insertIndex;
        private PlayerPrefsInt _fetchIndex;

        public int Count => _insertIndex.Value == INSERT_INDEX_FULL
            ? Length
            : (_insertIndex.Value > _fetchIndex.Value
                ? _insertIndex.Value - _fetchIndex.Value
                : Length - _insertIndex.Value - (Length - _fetchIndex.Value));

        /// <summary>
        /// Initializes a new queue
        /// </summary>
        /// <param name="keyPrefix">The prefix to use when reading and writing PlayerPref values</param>
        /// <param name="length">The maximum amount of items stored in the queue</param>
        public PlayerPrefsQueue(string keyPrefix, int length)
        {
            _keyPrefix = keyPrefix;
            Length = length;
            _insertIndex = new PlayerPrefsInt(keyPrefix + ":insertIndex", 0);
            _fetchIndex = new PlayerPrefsInt(keyPrefix + ":fetchIndex", 0);
        }
        
        /// <summary>
        /// Enqueue a new object in the queue
        /// </summary>
        /// <param name="data">The object to be queued</param>
        /// <exception cref="QueueFullException">Raised if there is no space left in the queue</exception>
        public void Enqueue(K data)
        {
            if (_insertIndex.Value == INSERT_INDEX_FULL)
            {
                throw new QueueFullException();
            }

            var itemPref = new T();
            itemPref.Initialize(_keyPrefix + ":item:" + _insertIndex.Value.ToString(), default(K));
            itemPref.Value = data;
            _insertIndex.Value = GetNextIndex(_insertIndex.Value);
            if (_insertIndex.Value == _fetchIndex.Value)
            {
                _insertIndex.Value = INSERT_INDEX_FULL;
            }
        }

        /// <summary>
        /// Dequeues the next object from the queue
        /// </summary>
        /// <returns>The queued object</returns>
        /// <exception cref="QueueEmptyException">Raised if there are no objects left in the queue</exception>
        public K Dequeue()
        {
            if (_insertIndex == _fetchIndex)
            {
                throw new QueueEmptyException();
            }

            var itemPref = new T();
            itemPref.Initialize(_keyPrefix + ":item:" + _insertIndex.Value.ToString(), default(K));
            var result = itemPref.Value;
            itemPref.Delete();
            _fetchIndex.Value = GetNextIndex(_fetchIndex.Value);
            return result;
        }

        private int GetNextIndex(int index)
        {
            return (index + 1) % Length;
        }
    }
}