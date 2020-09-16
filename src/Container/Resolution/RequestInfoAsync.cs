using Unity.Resolution;

namespace Unity.Container
{

    /// <summary>
    /// Internal state passed to <see cref="ResolveAsync"/>
    /// </summary>
    internal class RequestInfoAsync
    {
        public readonly RequestInfo Info;
        public readonly Contract Contract;
        public readonly RegistrationManager? Manager;

        public RequestInfoAsync(in Contract contract, ResolverOverride[] overrides)
        {
            Info = new RequestInfo(overrides);
            Contract = contract;
            Manager = null;
        }

        public RequestInfoAsync(in Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            Info = new RequestInfo(overrides);
            Contract = contract;
            Manager = manager;
        }
    }
}
