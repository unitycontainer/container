using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Lifetime
{
    public class LifetimeContainer : List<IDisposable>
#if UNITY_V5
        , ILifetimeContainer
#endif
    {
        public IUnityContainer Container => throw new NotImplementedException();

        public void Add(object item)
        {
            if (item is IDisposable disposable)
                base.Add(disposable);
        }

        public bool Contains(object item)
        {
            return item is IDisposable disposable
                ? base.Remove(disposable)
                : false;
        }

        public void Dispose() => Clear();

        public new IEnumerator<object> GetEnumerator()
            => base.GetEnumerator();

        public void Remove(object item)
        {
            if (item is IDisposable disposable)
                base.Remove(disposable);
        }

#if UNITY_V5
        IEnumerator IEnumerable.GetEnumerator()
            => base.GetEnumerator();
#endif
    }
}
