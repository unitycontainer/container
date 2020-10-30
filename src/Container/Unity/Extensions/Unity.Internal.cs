using System;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

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
}
