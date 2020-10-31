using System;

namespace Unity
{
    internal static partial class UnityContainerInternalExtensions
    {
        public static UnityContainer[] CreateClone(this UnityContainer[] parent, UnityContainer child)
        {
            var ancestry = new UnityContainer[parent.Length + 1];
            ancestry[parent.Length] = child;
            parent.CopyTo(ancestry, 0);

            return ancestry;
        }


    }

#if !NET5_0

    [AttributeUsage(AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface, Inherited = false)]
    internal sealed class SkipLocalsInitAttribute : Attribute
    {
    }
    
#endif

}
