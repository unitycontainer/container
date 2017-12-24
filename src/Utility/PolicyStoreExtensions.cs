using Unity.Policy;

namespace Unity.Storage
{
    public static class PolicyStoreExtensions
    {
        public static TInterface Get<TInterface>(this IPolicyStore store)
            where TInterface : IBuilderPolicy
        {
            return (TInterface) store.Get(typeof(TInterface));
        }

        public static void Set<TInterface>(this IPolicyStore store, IBuilderPolicy policy)
            where TInterface : IBuilderPolicy
        {
            store.Set(typeof(TInterface), policy);
        }


        public static void Clear<TInterface>(this IPolicyStore store)
            where TInterface : IBuilderPolicy
        {
            store.Clear(typeof(TInterface));
        }

    }
}
