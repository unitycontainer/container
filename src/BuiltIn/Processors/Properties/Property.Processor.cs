using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor : MemberProcessor<PropertyInfo, PropertyInfo, object>
    {
        #region Constructors

        /// <inheritdoc/>
        public PropertyProcessor(Defaults defaults)
            : base(defaults, (member) => member.PropertyType,
                             (member) => member.DeclaringType!,
                               (type) => type.GetProperties(BindingFlags.Public | BindingFlags.Instance),
                               DefaultImportProvider)
        {
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjectedMembers<TMember>(RegistrationManager? registration) 
            where TMember : class => Unsafe.As<TMember>(registration?.Properties);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(PropertyInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
