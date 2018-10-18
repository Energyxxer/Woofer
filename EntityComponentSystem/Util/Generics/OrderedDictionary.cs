using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponentSystem.Util.Generics
{
    public class OrderedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly List<TKey> Indices = new List<TKey>();
        private readonly Dictionary<TKey, TValue> Lookup = new Dictionary<TKey, TValue>();

        public IEnumerable<TKey> Keys => Indices.ToList();
        public IEnumerable<TValue> Values => Indices.Select(k => Lookup[k]);
        public int Count => Indices.Count;

        public TValue this[TKey key] => Lookup[key];
        public TValue this[int index] => Lookup[Indices[index]];

        public OrderedDictionary()
        {
        }

        public bool ContainsKey(TKey key)
        {
            return Indices.Contains(key);
        }

        public bool ContainsValue(TValue value)
        {
            return Lookup.ContainsValue(value);
        }

        public void Add(TKey key, TValue value)
        {
            Lookup[key] = value;
            if (ContainsKey(key))
            {
                Indices.Remove(key);
            }
            Indices.Add(key);
        }

        public void Insert(TKey key, TValue value, int index)
        {
            if(ContainsKey(key))
            {
                Indices.Remove(key);
            }
            Indices.Insert(index, key);
            Lookup[key] = value;
        }

        public void Clear()
        {
            Indices.Clear();
            Lookup.Clear();
        }

        public bool Remove(TKey key)
        {
            if(ContainsKey(key))
            {
                Indices.Remove(key);
                Lookup.Remove(key);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            Lookup.Remove(Indices[index]);
            Indices.RemoveAt(index);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if(ContainsKey(key))
            {
                value = Lookup[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Indices.Select(k => new KeyValuePair<TKey, TValue>(k, Lookup[k])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
