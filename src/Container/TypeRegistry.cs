using System;
using Unity.Container.Registration;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container
{
    public class TypeRegistry : IPolicyList
    {
        #region Constants

        private const float LoadFactor = 0.72f;
        private const int InitialCapacity = 37;
        private const int CutoverPoint = 8;
        private static readonly int[] Primes = {
            1, 3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293,
            353, 431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049,
            4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229, 30293, 36353,
            43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751, 225307, 270371,
            324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 1674319,
            2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369, 10000019};

        #endregion


        #region Fields

        private readonly object _syncRoot = new object();
        private readonly TypeRegistry _parent;
        private HashMicroSet<Type, IMicroSet<string, IContainerRegistration>> _registrations;

        #endregion


        #region Constructors

        public TypeRegistry(TypeRegistry parent = null)
        {
            _parent = parent;
            _registrations = new HashMicroSet<Type, IMicroSet<string, IContainerRegistration>>(InitialCapacity);
        }

        #endregion


        #region Public Members

        public void Register(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            Register(new TypeRegistration(this, typeFrom, typeTo, name, lifetimeManager, injectionMembers)); 
        }

        public void Register(Type mapType, string name, object instance, LifetimeManager lifetime)
        {
            Register(new TypeRegistration(this, typeFrom, typeTo, name, lifetimeManager, injectionMembers));
        }

        #endregion


        #region IPolicyList

        public IBuilderPolicy Get(Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            throw new NotImplementedException();
        }

        public void Set(Type policyInterface, IBuilderPolicy policy, object buildKey = null)
        {
            throw new NotImplementedException();
        }

        public void Clear(Type policyInterface, object buildKey)
        {
            throw new NotImplementedException();
        }

        public void ClearAll()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Implementation

        private void Register(IContainerRegistration registration)
        {
            lock (_syncRoot)
            {
                var key = registration.RegisteredType;
                var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
                var targetBucket = hashCode % _registrations.Buckets.Length;
                var collisions = 0;
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    if (_registrations.Entries[i].HashCode != hashCode ||
                        _registrations.Entries[i].Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    if (_registrations.Entries[i].Value.RequireToGrow)
                    {
                        // TODO: lock (entry.Key)
                        {
                            _registrations.Entries[i].Value = Resize(_registrations.Entries[i].Value);
                        }
                    }

                    _registrations.Entries[i].Value[registration.Name] = registration;

                    return;
                }

                if (_registrations.RequireToGrow || CutoverPoint < collisions)
                {
                    _registrations = new HashMicroSet<Type, IMicroSet<string, IContainerRegistration>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = key;
                _registrations.Entries[_registrations.Count].Value = new ListMicroSet(registration);
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;
            }
        }

        private IMicroSet<string, IContainerRegistration> Resize(IMicroSet<string, IContainerRegistration> dictionary)
        {
            if (dictionary is ListMicroSet list)
            {
                var newDictionary = new HashMicroSet<string, IContainerRegistration>(11);
                for (LinkedNode node = list.Head; null != node; node = node.Next)
                {
                    newDictionary[node.Key] = node.Value;
                }
                return newDictionary;
            }

            return new HashMicroSet<string, IContainerRegistration>(
                (HashMicroSet<string, IContainerRegistration>)dictionary);
        }

        private static int GetPrime(int min)
        {
            if (min < 0) throw new ArgumentException("Capacity Overflow");

            foreach (var prime in Primes)
            {
                if (prime >= min) return prime;
            }

            for (var i = (min | 1); i < Int32.MaxValue; i += 2)
            {
                if (IsPrime(i) && (i - 1) % 101 != 0)
                    return i;
            }

            return min;
        }

        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int limit = (int)Math.Sqrt(candidate);
                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if (candidate % divisor == 0)
                        return false;
                }
                return true;
            }
            return (candidate == 2);
        }

        #endregion


        #region Nested Types

        private interface IMicroSet<TKey, TValue>
        {
            TValue this[TKey index] { get; set; }

            bool RequireToGrow { get; }
        }


        public class LinkedNode
        {
            public string Key;
            public IContainerRegistration Value;
            public LinkedNode Next;
        }


        private class ListMicroSet : IMicroSet<string, IContainerRegistration>
        {
            #region Fields

            private int _count = 1;

            #endregion


            #region Constructors

            public ListMicroSet(IContainerRegistration value)
            {
                Head = new LinkedNode
                {
                    Key = value?.Name,
                    Value = value,
                    Next = null
                };
            }

            #endregion


            public LinkedNode Head;


            #region IMicroSet

            public IContainerRegistration this[string key]
            {
                get
                {
                    if (key == null)
                    {
                        throw new ArgumentNullException(nameof(key));
                    }

                    var node = Head;

                    while (node != null)
                    {
                        string oldKey = node.Key;
                        if (Equals(oldKey, key))
                        {
                            return node.Value;
                        }
                        node = node.Next;
                    }

                    return null;
                }
                set
                {
                    LinkedNode node;
                    LinkedNode last = null;

                    for (node = Head; node != null; node = node.Next)
                    {
                        string oldKey = node.Key;
                        if (Equals(oldKey, key))
                        {
                            break;
                        }
                        last = node;
                    }

                    if (node != null)
                    {
                        // Found it
                        node.Value = value;
                        return;
                    }

                    // Not found, so add a new one
                    var newNode = new LinkedNode
                    {
                        Key = key,
                        Value = value
                    };

                    if (last != null)
                        last.Next = newNode;
                    else
                        Head = newNode;

                    _count++;
                }
            }

            public bool RequireToGrow => CutoverPoint < _count;

            #endregion
        }


        private class HashMicroSet<TKey, TValue> : IMicroSet<TKey, TValue>
        {
            #region Fields

            public readonly int[] Buckets;
            public readonly Entry[] Entries;
            public int Count = 0;

            #endregion


            #region Constructors

            public HashMicroSet(int capacity)
            {
                var size = GetPrime(capacity);
                Buckets = new int[size];
                Entries = new Entry[size];

                for (var i = 0; i < Buckets.Length; i++) Buckets[i] = -1;
            }

            public HashMicroSet(HashMicroSet<TKey, TValue> dictionary)
            {
                var size = GetPrime(dictionary.Entries.Length * 2);

                Buckets = new int[size];
                Entries = new Entry[size];
                for (var i = 0; i < Buckets.Length; i++) Buckets[i] = -1;

                Array.Copy(dictionary.Entries, 0, Entries, 0, dictionary.Count);
                for (var i = 0; i < dictionary.Count; i++)
                {
                    var hashCode = Entries[i].HashCode;
                    if (hashCode < 0) continue;

                    var bucket = hashCode % size;
                    Entries[i].Next = Buckets[bucket];
                    Buckets[bucket] = i;
                }
                Count = dictionary.Count;
                dictionary.Count = 0;
            }

            #endregion


            #region IMicroSet

            public TValue this[TKey key]
            {
                get
                {
                    var hashCode = null == key ? 0 : key.GetHashCode() & 0x7FFFFFFF;
                    for (var i = Buckets[hashCode % Buckets.Length]; i >= 0; i = Entries[i].Next)
                    {
                        if (Entries[i].HashCode == hashCode && Equals(Entries[i].Key, key)) return Entries[i].Value;
                    }

                    return default(TValue);
                }

                set
                {
                    var hashCode = null == key ? 0 : key.GetHashCode() & 0x7FFFFFFF;
                    var targetBucket = hashCode % Buckets.Length;

                    for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
                    {
                        if (Entries[i].HashCode == hashCode && Equals(Entries[i].Key, key))
                        {
                            Entries[i].Value = value;
                            return;
                        }
                    }

                    Entries[Count].HashCode = hashCode;
                    Entries[Count].Next = Buckets[targetBucket];
                    Entries[Count].Key = key;
                    Entries[Count].Value = value;
                    Buckets[targetBucket] = Count;
                    Count++;
                }
            }

            public bool RequireToGrow => (Entries.Length - Count) < 100 &&
                                         (float)Count / Entries.Length > LoadFactor;

            #endregion


            #region Nested Types

            public struct Entry
            {
                public int HashCode;
                public int Next;
                public TKey Key;
                public TValue Value;
            }

            #endregion
        }

        #endregion
    }
}
