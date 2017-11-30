using System;
using Unity.Container.Storage;
using Unity.Policy;

namespace Unity.Container.Registration
{
    public class PolicyRegistry : IRegistry<Type, IBuilderPolicy>
    {
        #region Fields

        private LinkedNode<Type, IBuilderPolicy> _head;

        #endregion


        public virtual IBuilderPolicy this[Type policy]
        {
            get
            {
                for (var node = _head; node != null; node = node.Next)
                {
                    if (Equals(node.Key, policy))
                        return node.Value;
                }

                return null;
            }
            set
            {
                for (var node = _head; node != null; node = node.Next)
                {
                    if (Equals(node.Key, policy))
                    {
                        // Found it
                        node.Value = value;
                        return;
                    }
                }

                _head = new LinkedNode<Type, IBuilderPolicy>
                {
                    Key = policy,
                    Next = _head,
                    Value = value
                };
            }
        }
    }
}
