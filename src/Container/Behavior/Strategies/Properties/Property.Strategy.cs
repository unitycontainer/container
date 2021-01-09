using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public partial class PropertyStrategy : MemberStrategy<PropertyInfo, PropertyInfo, object>
    {
        #region Constructors

        /// <inheritdoc/>
        public PropertyStrategy(IPolicies policies)
            : base(policies)
        { }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(PropertyInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
