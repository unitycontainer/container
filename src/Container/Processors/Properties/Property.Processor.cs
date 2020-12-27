using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public partial class PropertyProcessor : MemberProcessor<PropertyInfo, PropertyInfo, object>
    {
        #region Constructors

        static PropertyProcessor()
        {
            GetMemberType = (member) => member.PropertyType;
            GetDeclaringType = (member) => member.DeclaringType!;
        }

        /// <inheritdoc/>
        public PropertyProcessor(IPolicies policies)
            : base(policies) 
            => policies.Set<ImportProvider<ImportInfo, ImportType>>(typeof(PropertyInfo), DefaultImportProvider);

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(PropertyInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
