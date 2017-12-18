using System;
using System.Collections.Generic;
using Unity.Policy;

namespace Unity.Storage
{

    internal class LinkedRegistry : LinkedNode<string, IPolicyStore>, 
                                    IRegistry<string, IPolicyStore>
    {
        #region Fields

        private int _count;
        public const int ListToHashCutoverPoint = 8;

        #endregion

        
        #region IRegistry

        public IPolicyStore this[string key]
        {
            get
            {
                for (var node = (LinkedNode<string, IPolicyStore>)this; node != null; node = node.Next)
                {
                    if (Equals(node.Key, key))
                        return node.Value;
                }

                return default(IPolicyStore);
            }
            set
            {
                LinkedNode<string, IPolicyStore> node;
                LinkedNode<string, IPolicyStore> last = null;

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
                last.Next = new LinkedNode<string, IPolicyStore>
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
                for (LinkedNode<string, IPolicyStore> node = this; node != null; node = node.Next)
                {
                    yield return node.Key;
                }
            }
        }

        public IEnumerable<IPolicyStore> Values
        {
            get
            {
                for (LinkedNode<string, IPolicyStore> node = this; node != null; node = node.Next)
                {
                    yield return node.Value;
                }
            }
        }

        public IPolicyStore GetOrAdd(string name, Func<IPolicyStore> factory)
        {
            LinkedNode<string, IPolicyStore> node;
            LinkedNode<string, IPolicyStore> last = null;

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
            last.Next = new LinkedNode<string, IPolicyStore>
            {
                Key = name,
                Value = factory()
            };

            _count++;

            return last.Next.Value;
        }

        public IPolicyStore SetOrReplace(string name, IPolicyStore value)
        {
            LinkedNode<string, IPolicyStore> node;
            LinkedNode<string, IPolicyStore> last = null;

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
            last.Next = new LinkedNode<string, IPolicyStore>
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
