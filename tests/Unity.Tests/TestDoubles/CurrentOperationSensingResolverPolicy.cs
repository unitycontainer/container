using Unity.Builder;
using Unity.Policy;

namespace Unity.Tests.v5.TestDoubles
{
    public class CurrentOperationSensingResolverPolicy<T> : IResolverPolicy
    {
        public object CurrentOperation;

        public object Resolve<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            this.CurrentOperation = context.CurrentOperation;

            return default(T);
        }
    }
}
