using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <summary>
        /// Silently resolve registration
        /// </summary>
        private object? ResolveRegistered(ref Contract contract, RegistrationManager manager)
        {
            var request = new RequestInfo(new ResolverOverride[0]);
            var context = new BuilderContext(this, ref contract, manager, ref request);

            Policies.ResolveRegistered(ref context);

            return request.IsFaulted ? null : context.Target;
        }


        /// <summary>
        /// Silently resolve unknown type
        /// </summary>
        private object? ResolveUnregistered(ref Contract contract)
        {
            // TODO: Replace empty overrides with 'null'
            var request = new RequestInfo(new ResolverOverride[0]);
            var value = ResolveUnregistered(ref contract, ref request);

            return request.IsFaulted ? null : value;
        }
    }
}
