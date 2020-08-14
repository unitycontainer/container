using System;
using System.Runtime.InteropServices;
using Unity.Resolution;

namespace Unity.Container
{
    [StructLayout(LayoutKind.Sequential)]
    public ref struct PipelineContext
    {
        public Contract Contract;
        public UnityContainer? Container;
        public RegistrationManager? Manager;
        public ResolverOverride[] Overrides;
        public object? Existing;

        readonly Span<ResolveContext> Parent;
    }
}
