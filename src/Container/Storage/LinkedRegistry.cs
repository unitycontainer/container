using System;
using Unity.Container.Registration;
using Unity.Policy;

namespace Unity.Container.Storage
{

    internal class LinkedRegistry : LinkedMap<string, IMap<Type, IBuilderPolicy>>, 
                                    IRegistry<string, IMap<Type, IBuilderPolicy>>
    {
        #region Fields

        private int _count;

        #endregion


        public const int ListToHashCutoverPoint = 8;


        #region IRegistry

        public IMap<Type, IBuilderPolicy> GetOrAdd(string name, Func<IMap<Type, IBuilderPolicy>> factory)
        {
            LinkedNode<string, IMap<Type, IBuilderPolicy>> node;
            LinkedNode<string, IMap<Type, IBuilderPolicy>> last = null;

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
            last.Next = new LinkedNode<string, IMap<Type, IBuilderPolicy>>
            {
                Key = name,
                Value = factory()
            };

            _count++;

            return last.Next.Value;
        }

        public IMap<Type, IBuilderPolicy> SetOrReplace(string name, IMap<Type, IBuilderPolicy> value)
        {
            LinkedNode<string, IMap<Type, IBuilderPolicy>> node;
            LinkedNode<string, IMap<Type, IBuilderPolicy>> last = null;

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
            last.Next = new LinkedNode<string, IMap<Type, IBuilderPolicy>>
            {
                Key = name,
                Value = value
            };

            _count++;

            return null;
        }

        public override IMap<Type, IBuilderPolicy> this[string key]
        {
            set
            {
                LinkedNode<string, IMap<Type, IBuilderPolicy>> node;
                LinkedNode<string, IMap<Type, IBuilderPolicy>> last = null;

                for (node = this; node != null; node = node.Next)
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
                last.Next = new LinkedNode<string, IMap<Type, IBuilderPolicy>>
                {
                    Key = key,
                    Value = value
                };

                _count++;
            }
        }

        public bool RequireToGrow => ListToHashCutoverPoint < _count;

        #endregion
    }
}
