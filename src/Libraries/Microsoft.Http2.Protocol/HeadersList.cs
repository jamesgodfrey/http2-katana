﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Microsoft.Http2.Protocol
{
    /// <summary>
    /// Headers list class.
    /// </summary>
    public class HeadersList : IList<KeyValuePair<string, string>>
    {
        private readonly List<KeyValuePair<string, string>> _collection;
        private readonly object _modificationLock = new object();
        /// <summary>
        /// Gets the size of the stored headers in bytes.
        /// </summary>
        /// <value>
        /// The size of the stored headers in bytes.
        /// </value>
        public int StoredHeadersSize { get; private set; }

        public HeadersList() 
            :this(16)
        { }

        public HeadersList(IEnumerable<KeyValuePair<string, string>> list)
        {
            _collection = new List<KeyValuePair<string, string>>();
            AddRange(list);
        }

        public HeadersList(IDictionary<string, string[]> headers)
        {
            _collection = new List<KeyValuePair<string, string>>();

            //Send only first value?
            foreach (var header in headers)
            {
                _collection.Add(new KeyValuePair<string, string>(header.Key.ToLower(), header.Value[0].ToLower()));
            }
        }

        public HeadersList(int capacity)
        {
            _collection = new List<KeyValuePair<string, string>>(capacity);
        }

        public string GetValue(string key)
        {
            var headerFound = _collection.Find(header => header.Key == key);

            if (!headerFound.Equals(default(KeyValuePair<string, string>)))
            {
                return headerFound.Value;
            }

            return null;
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> headers)
        {
            lock (_modificationLock)
            {
                foreach (var header in headers)
                {
                    Add(header);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> header)
        {
            lock (_modificationLock)
            {
                _collection.Add(header);
                StoredHeadersSize += header.Key.Length + header.Value.Length + sizeof (Int32);
            }
        }

        public void Clear()
        {
            lock (_modificationLock)
            {
                _collection.Clear();
                StoredHeadersSize = 0;
            }
        }

        public bool Contains(KeyValuePair<string, string> header)
        {
            return _collection.Contains(header);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            Contract.Assert(arrayIndex >= 0 && arrayIndex < Count && array != null);
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string> header)
        {
            lock (_modificationLock)
            {
                StoredHeadersSize -= header.Key.Length + header.Value.Length + sizeof (Int32);
                return _collection.Remove(header);
            }
        }

        public int Count
        {
            get { return _collection.Count; }
        }
    
        public bool IsReadOnly
        {
            get { return true; }
        }

        public int FindIndex(Predicate<KeyValuePair<string, string>> predicate)
        {
            return _collection.FindIndex(predicate);
        }

        public int IndexOf(KeyValuePair<string, string> header)
        {
            return _collection.IndexOf(header);
        }

        public void Insert(int index, KeyValuePair<string, string> header)
        {
            lock (_modificationLock)
            {
                Contract.Assert(index >= 0 && index < Count);
                StoredHeadersSize += header.Key.Length + header.Value.Length + sizeof (Int32);
                _collection.Insert(index, header);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_modificationLock)
            {
                Contract.Assert(index >= 0 && index < Count);
                var header = _collection[index];
                _collection.RemoveAt(index);
                StoredHeadersSize -= header.Key.Length + header.Value.Length + sizeof (Int32);
            }
        }

        public int RemoveAll(Predicate<KeyValuePair<string,string>> predicate)
        {
            lock (_modificationLock)
            {

                var predMatch = _collection.FindAll(predicate);
                int toDeleteSize = predMatch.Sum(header => header.Key.Length + header.Value.Length + sizeof (Int32));
                StoredHeadersSize -= toDeleteSize;

                return _collection.RemoveAll(predicate);
            }
        }

        public KeyValuePair<string, string> this[int index]
        {
            get
            {
                Contract.Assert(index >= 0 && index < Count);
                return _collection[index];
            }
            set
            {
                lock (_modificationLock)
                {
                    Contract.Assert(index >= 0 && index < Count);
                    _collection[index] = value;
                }
            }
        }
    }
}
