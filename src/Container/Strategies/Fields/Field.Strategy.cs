using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public partial class FieldStrategy : MemberStrategy<FieldInfo, FieldInfo, object>
    {
        #region Constructors

        /// <inheritdoc/>
        public FieldStrategy(IPolicies policies)
            : base(policies)
        { }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void Execute(FieldInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
