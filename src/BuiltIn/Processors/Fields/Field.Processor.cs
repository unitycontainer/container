using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, FieldInfo, object>
    {
        #region Constructors

        /// <inheritdoc/>
        public FieldProcessor(Defaults defaults)
            : base(defaults, (member) => member.FieldType,
                             (member) => member.DeclaringType!,
                               (type) => type.GetFields(BindingFlags.Public | BindingFlags.Instance),
                               DefaultImportProvider)
        {
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjectedMembers<TMember>(RegistrationManager? registration)
            where TMember : class => Unsafe.As<TMember>(registration?.Fields);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(FieldInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
