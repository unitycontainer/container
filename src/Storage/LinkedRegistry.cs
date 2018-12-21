using System;
using System.Collections.Generic;

namespace Unity.Storage
{

    internal class LinkedRegistry : LinkedNode<string, IPolicySet>, 
                                    IRegistry<string, IPolicySet>
    {
        #region Fields

        private int _count;
        public const int ListToHashCutoverPoint = 8;

        #endregion


        #region Constructors

        public LinkedRegistry(string key, IPolicySet value)
        {
            Key = key;
            Value = value;
        }

        #endregion


        #region IRegistry

        public IPolicySet this[string key]
        {
            get
            {
                for (var node = (LinkedNode<string, IPolicySet>)this; node != null; node = node.Next)
                {
                    if (Equals(node.Key, key))
                        return node.Value;
                }

                return default(IPolicySet);
            }
            set
            {
                LinkedNode<string, IPolicySet> node;
                LinkedNode<string, IPolicySet> last = null;

                for (node = this; node != null; node = node.Next)
                {
                    if (Equals(node.Key, key))
                    {
                        // Found it
                        node.Value = value;
                        return;
                    }
                    last = node;
                }

                // Not found, so add a new one
                last.Next = new LinkedNode<string, IPolicySet>
                {
                    Key = key,
                    Value = value
                };

                _count++;
            }
        }

        public bool RequireToGrow => ListToHashCutoverPoint < _count;

        public IEnumerable<string> Keys
        {
            get
            {
                for (LinkedNode<string, IPolicySet> node = this; node != null; node = node.Next)
                {
                    yield return node.Key;
                }
            }
        }

        public IEnumerable<IPolicySet> Values
        {
            get
            {
                for (LinkedNode<string, IPolicySet> node = this; node != null; node = node.Next)
                {
                    yield return node.Value;
                }
            }
        }

        public IPolicySet GetOrAdd(string name, Func<IPolicySet> factory)
        {
            LinkedNode<string, IPolicySet> node;
            LinkedNode<string, IPolicySet> last = null;

            for (node = this; node != null; node = node.Next)
            {
                if (Equals(node.Key, name))
                {
                    if (null == node.Value)
                        node.Value = factory();

                    return node.Value;
                }
                last = node;
            }

            // Not found, so add a new one
            last.Next = new LinkedNode<string, IPolicySet>
            {
                Key = name,
                Value = factory()
            };

            _count++;

            return last.Next.Value;
        }

        public IPolicySet SetOrReplace(string name, IPolicySet value)
        {
            LinkedNode<string, IPolicySet> node;
            LinkedNode<string, IPolicySet> last = null;

            for (node = this; node != null; node = node.Next)
            {
                if (Equals(node.Key, name))
                {
                    var old = node.Value;
                    node.Value = value;
                    return old;
                }
                last = node;
            }

            // Not found, so add a new one
            last.Next = new LinkedNode<string, IPolicySet>
            {
                Key = name,
                Value = value
            };

            _count++;

            return null;
        }


        #endregion
    }
}
