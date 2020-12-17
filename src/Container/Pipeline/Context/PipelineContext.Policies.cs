using System;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        #region Property

        /// <inheritdoc/>
        public IPolicyList Policies => Container._policies;

        #endregion


        #region IPolicyList

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Get(Type? type, Type policy)
            => Container._policies.Get(type, policy);


        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Type? type, Type policy, object instance)
            => Container._policies.Set(type, policy, instance);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(Type? type, Type policy)
            => Container._policies.Clear(type, policy);

        #endregion


        #region IPolicySet

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(Type type)
            => Container._policies.Clear(type);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Get(Type type)
            => Container._policies.Get(type);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Type type, object policy)
            => Container._policies.Set(type, policy);

        #endregion
    }
}
