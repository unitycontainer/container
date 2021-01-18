using System;
using System.Diagnostics;

namespace Unity.Container
{
    public partial class Policies<TContext>
    {
        #region Build Chains

        public StagedStrategyChain TypeChain { get; }

        public StagedStrategyChain FactoryChain { get; }

        public StagedStrategyChain InstanceChain { get; }

        public StagedStrategyChain MappingChain { get; }

        #endregion


        #region Public Members

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ReadOnlySpan<Policy> Span => new ReadOnlySpan<Policy>(Data, 1, Count);

        public bool Contains(Type? target, Type type)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            var position = Meta[hash % Meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return true;
                }

                position = Meta[position].Location;
            }

            return false;
        }

        #endregion


        #region Marker Types

        /// <summary>
        /// Type identifying <see cref="RegistrationCategory.Instance"/> policies
        /// </summary>
        public class CategoryInstance
        { }

        #endregion
    }
}
