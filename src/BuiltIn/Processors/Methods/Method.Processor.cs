using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(Defaults defaults)
            : base(defaults, GetMethods)
        {
        }

        #endregion


        #region Implementation

        private static MethodInfo[] GetMethods(Type type) => type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjected<TMember>(RegistrationManager? registration)
            where TMember : class => Unsafe.As<TMember>(registration?.Methods);

        #endregion
    }
}
