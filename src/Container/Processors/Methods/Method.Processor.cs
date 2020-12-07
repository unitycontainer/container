using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(Defaults defaults)
            : base(defaults, (Type type) => type.GetMethods(BindingFlags.Public   | 
                                                            BindingFlags.Instance | 
                                                            BindingFlags.FlattenHierarchy |
                                                            BindingFlags.DeclaredOnly))
        {
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjectedMembers<TMember>(RegistrationManager? registration)
            where TMember : class => Unsafe.As<TMember>(registration?.Methods);

        #endregion
    }
}
