using System;
using System.ComponentModel;
using Unity.Extensions.Syntax;
using Unity.Lifetime;
using static Unity.Extensions.Syntax.TypeRegistration;


namespace Unity
{

    public static class TypeRegistrationLifetimeExtension
    {
        public static TypeRegistration SingletonLifetime(this LifetimeProxy proxy)
        {
            return proxy.WithLifetime(new SingletonLifetimeManager());
        }

        public static TypeRegistration PerContainerLifetime(this LifetimeProxy proxy)
        {
            return proxy.WithLifetime(new ContainerControlledLifetimeManager());
        }

        public static TypeRegistration ContainerControlledLifetime(this LifetimeProxy proxy)
        {
            return proxy.WithLifetime(new ContainerControlledLifetimeManager());
        }

        public static TypeRegistration HierarchicalLifetime(this LifetimeProxy proxy)
        {
            return proxy.WithLifetime(new HierarchicalLifetimeManager());
        }

        public static TypeRegistration PerResolveLifetime(this LifetimeProxy proxy)
        {
            return proxy.WithLifetime(new PerResolveLifetimeManager());
        }

        public static TypeRegistration PerThreadLifetime(this LifetimeProxy proxy)
        {
            return proxy.WithLifetime(new PerThreadLifetimeManager());
        }

        public static TypeRegistration PerContainerTransientLifetime(this LifetimeProxy proxy)
        {
            return proxy.WithLifetime(new ContainerControlledTransientManager());
        }

    }

    public class LifetimeProxy
    {
        TypeRegistration _parent;

        internal LifetimeProxy(TypeRegistration parent)
        {
            _parent = parent;
        }

        public TypeRegistration WithLifetime(ITypeLifetimeManager manager)
        {
            _parent.lifetime = manager;
            return _parent;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
