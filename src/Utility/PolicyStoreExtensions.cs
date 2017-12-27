using Unity.Policy;

namespace Unity.Storage
{
    public static class PolicyStoreExtensions
    {
        public static TInterface Get<TInterface>(this IPolicySet set)
            where TInterface : IBuilderPolicy
        {
            return (TInterface) set.Get(typeof(TInterface));
        }

        public static void Set<TInterface>(this IPolicySet set, IBuilderPolicy policy)
            where TInterface : IBuilderPolicy
        {
            set.Set(typeof(TInterface), policy);
        }


        public static void Clear<TInterface>(this IPolicySet set)
            where TInterface : IBuilderPolicy
        {
            set.Clear(typeof(TInterface));
        }

    }
}
