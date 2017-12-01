using System;
using Unity.Container.Registration;
using Unity.Policy;

namespace Unity.Container.Storage
{

    internal class ListRegistry : IRegistry<string, IMap<Type, IBuilderPolicy>>
    {
        #region Fields

        private int _count;

        #endregion


        #region Constructors

        public ListRegistry()
        {
            
        }

        public ListRegistry(string name, IMap<Type, IBuilderPolicy> value)
        {
            _count += 1;
            Head = new LinkedNode<string, IMap<Type, IBuilderPolicy>>
            {
                Key = name,
                Value = value,
                Next = null
            };
        }

        #endregion


        public const int ListToHashCutoverPoint = 8;

        public LinkedNode<string, IMap<Type, IBuilderPolicy>> Head;


        #region IRegistry

        public IMap<Type, IBuilderPolicy> GetOrAdd(string name, Func<IMap<Type, IBuilderPolicy>> factory)
        {
            LinkedNode<string, IMap<Type, IBuilderPolicy>> node;
            LinkedNode<string, IMap<Type, IBuilderPolicy>> last = null;

            for (node = Head; node != null; node = node.Next)
            {
                if (Equals(node.Key, name))
                {
                    // Found it
                    return node.Value;
                }
                last = node;
            }

            // Not found, so add a new one
            var newNode = new LinkedNode<string, IMap<Type, IBuilderPolicy>>
            {
                Key = name,
                Value = new PolicyRegistry()
            };

            if (last != null)
                last.Next = newNode;
            else
                Head = newNode;

            _count++;

            return newNode.Value;
        }

        public IMap<Type, IBuilderPolicy> SetOrReplace(string name, IMap<Type, IBuilderPolicy> value)
        {
            LinkedNode<string, IMap<Type, IBuilderPolicy>> node;
            LinkedNode<string, IMap<Type, IBuilderPolicy>> last = null;

            for (node = Head; node != null; node = node.Next)
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
            var newNode = new LinkedNode<string, IMap<Type, IBuilderPolicy>>
            {
                Key = name,
                Value = value
            };

            if (last != null)
                last.Next = newNode;
            else
                Head = newNode;

            _count++;

            return null;
        }

        public IMap<Type, IBuilderPolicy> this[string key]
        {
            get
            {
                var node = Head;

                while (node != null)
                {
                    if (Equals(node.Key, key))
                    {
                        return node.Value;
                    }
                    node = node.Next;
                }

                return null;
            }
            set
            {
                LinkedNode<string, IMap<Type, IBuilderPolicy>> node;
                LinkedNode<string, IMap<Type, IBuilderPolicy>> last = null;

                for (node = Head; node != null; node = node.Next)
                {
                    if (Equals(node.Key, key))
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
                var newNode = new LinkedNode<string, IMap<Type, IBuilderPolicy>>
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
