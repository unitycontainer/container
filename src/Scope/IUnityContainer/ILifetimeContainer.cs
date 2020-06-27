using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;

namespace Unity.Scope
{
    public partial class InjectionScope : ILifetimeContainer
    {
        public IUnityContainer Container => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public void Add(object item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(object item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
