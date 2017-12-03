using System;
using Unity.Policy;

namespace Unity.Container
{
    public interface IPolicyRegistry
    {
        IBuilderPolicy this[Type type, string name, Type policy] { get; set; }
    }


    public static class PolicyRegistryExtensions
    {
        public static T Get<T>(this IPolicyRegistry registry, Type type, string name)
        {
            return (T) registry[type, name, typeof(T)];
        }

        public static void Set<T>(this IPolicyRegistry registry, Type type, string name, IBuilderPolicy policy)
        {
            registry[type, name, typeof(T)] = policy;
        }
    }
}
