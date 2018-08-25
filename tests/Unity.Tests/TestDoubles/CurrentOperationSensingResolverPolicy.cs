using Unity.Builder;
using Unity.Policy;

namespace Unity.Tests.v5.TestDoubles
{
    public class CurrentOperationSensingResolverPolicy<T> : IResolverPolicy
    {
        public object CurrentOperation;

        public object Resolve(IBuilderContext context)
        {
            this.CurrentOperation = context.CurrentOperation;

            return default(T);
        }
    }
}
