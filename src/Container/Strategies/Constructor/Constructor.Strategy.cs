using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public partial class ConstructorStrategy : ParameterStrategy<ConstructorInfo>
    {
        #region Fields

        protected ConstructorSelector SelectAlgorithmically;

        #endregion


        #region Constructors

        public ConstructorStrategy(IPolicies policies)
            : base(policies)
        {
            SelectAlgorithmically = policies.Get<ConstructorSelector>(OnAlgorithmChanged)!;
        }

        #endregion


        #region Policy Changes

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnAlgorithmChanged(Type? target, Type type, object? policy) 
            => SelectAlgorithmically = (ConstructorSelector)(policy ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
