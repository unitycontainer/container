using System;

namespace Unity
{
#if NETSTANDARD

    [AttributeUsage(AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface, Inherited = false)]
    internal sealed class SkipLocalsInitAttribute : Attribute
    {
    }
    
#endif

}
