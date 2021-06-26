using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
using static Lifetime.Pattern;
#else
using Unity.Lifetime;
using static Lifetime.Pattern;
#endif

namespace Lifetime
{
    public class TestManagerSource
    {
        protected TestManagerSource(Type type,
            LifetimeManagerFactoryDelegate factory, 
            bool synchronized, 
            bool disposable,
            Type target,
            Action<object, object> set_get,
            Action<object, object> same_scope,
            Action<object, object> same_scope_threads,
            Action<object, object> child_scope,
            Action<object, object> child_scope_threads,
            Action<object, object> sibling_scope,
            Action<object, object> sibling_scope_threads
            )
        {
            Type = type;
            Target = target;
            
            Factory = factory;
            IsSynchronized = synchronized;
            IsDisposable = disposable;

            Assert_SetGet = set_get;

            Assert_SameScope = same_scope;
            Assert_SameScope_Threads = same_scope_threads;

            Assert_ChildScope = child_scope;
            Assert_ChildScope_Threads = child_scope_threads;

            Assert_SiblingScope = sibling_scope;
            Assert_SiblingScope_Threads = sibling_scope_threads;
        }

        public Type Type { get; }
        public string Name => Type.Name;

        public Type Target { get; }

        public virtual LifetimeManagerFactoryDelegate Factory { get; }

        public Action<object, object> Assert_SetGet { get; }

        public Action<object, object> Assert_SameScope { get; }
        public Action<object, object> Assert_SameScope_Threads { get; }

        public Action<object, object> Assert_ChildScope { get; }
        public Action<object, object> Assert_ChildScope_Threads { get; }

        public Action<object, object> Assert_SiblingScope { get; }
        public Action<object, object> Assert_SiblingScope_Threads { get; }

        public bool IsSynchronized { get; }
        public bool IsDisposable { get; }
    }

    public class TestManagerSource<T> : TestManagerSource
        where T : LifetimeManager
    {
        public TestManagerSource(LifetimeManagerFactoryDelegate factory, bool synchronized, bool disposable,
                                 Type target,
                                 Action<object, object> set_get,
                                 Action<object, object> same_scope,
                                 Action<object, object> same_scope_threads,
                                 Action<object, object> child_scope,
                                 Action<object, object> child_scope_threads,
                                 Action<object, object> sibling_scope,
                                 Action<object, object> sibling_scope_threads)
            : base(typeof(T), factory, synchronized, disposable, target, set_get, 
                  same_scope, same_scope_threads, 
                  child_scope, child_scope_threads,
                  sibling_scope, sibling_scope_threads)
        { }
    }
}
