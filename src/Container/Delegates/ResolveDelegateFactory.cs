using Unity.Resolution;

namespace Unity.Container
{
    public delegate ResolveDelegate<ResolveContext> ResolveDelegateFactory(in Contract contract, RegistrationManager? manager = null);
}
