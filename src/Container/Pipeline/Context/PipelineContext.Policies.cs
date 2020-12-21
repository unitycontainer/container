using System;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        #region Property

        /// <inheritdoc/>
        public IPolicyList Policies => Container.Policies;

        #endregion


        #region IPolicyList

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Get(Type? type, Type policy)
            => Container.Policies.Get(type, policy);


        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Type? type, Type policy, object instance)
            => Container.Policies.Set(type, policy, instance);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(Type? type, Type policy)
            => Container.Policies.Clear(type, policy);

        #endregion


        #region IPolicySet

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(Type type)
            => Container.Policies.Clear(type);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Get(Type type)
            => Container.Policies.Get(type);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Type type, object policy)
            => Container.Policies.Set(type, policy);

        #endregion
    }
}
