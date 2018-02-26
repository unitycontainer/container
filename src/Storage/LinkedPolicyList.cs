using System;
using System.Collections.Generic;
using System.Text;
using Unity.Policy;

namespace Unity.Storage
{
    public class LinkedPolicyList : LinkedNode<Type, IBuilderPolicy>, IPolicyList
    {
        public LinkedPolicyList(IUnityContainer container, LinkedNode<Type, IBuilderPolicy> parent)
        {
            Next = parent;
        }

        public void Clear(Type type, string name, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        public void ClearAll()
        {
            throw new NotImplementedException();
        }

        public IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
        {
            throw new NotImplementedException();
        }

        public void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
        {
            throw new NotImplementedException();
        }
    }
}
