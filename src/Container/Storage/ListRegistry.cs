using System;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container.Storage
{

    internal class LinkedNode
    {
        public string Key;
        public IRegistry<Type, IBuilderPolicy> Value;
        public LinkedNode Next;
    }


    internal class ListRegistry : IRegistry<string, IRegistry<Type, IBuilderPolicy>>
    {
        #region Fields

        private int _count = 1;

        #endregion


        #region Constructors

        public ListRegistry(IRegistry<Type, IBuilderPolicy> value)
        {
            Head = new LinkedNode
            {
                Key = (value as IContainerRegistration)?.Name,
                Value = value,
                Next = null
            };
        }

        #endregion


        public const int ListToHashCutoverPoint = 8;

        public LinkedNode Head;


        #region IRegistry

        public TValue GetOrAdd<TKey, TValue>(TKey key, Func<TValue> factory)
        {
            LinkedNode node;
            LinkedNode last = null;

            for (node = Head; node != null; node = node.Next)
            {
                string oldKey = node.Key;
                if (Equals(oldKey, key))
                {
                    // Found it
                    return (TValue)node.Value;
                }
                last = node;
            }

            // Not found, so add a new one
            var newNode = new LinkedNode
            {
                Key = key as string,
                Value = (IRegistry<Type, IBuilderPolicy>)factory()
            };

            if (last != null)
                last.Next = newNode;
            else
                Head = newNode;

            _count++;

            return (TValue)newNode.Value;
        }


        public IRegistry<Type, IBuilderPolicy> this[string key]
        {
            get
            {
                var node = Head;

                while (node != null)
                {
                    var oldKey = node.Key;
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

        public bool RequireToGrow => ListToHashCutoverPoint < _count;

        #endregion
    }
}
