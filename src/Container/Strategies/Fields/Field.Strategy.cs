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

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Execute<TContext, TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data)
        {
            if (!data.IsValue) return;

            descriptor.MemberInfo.SetValue(context.Existing, data.Value);
        }

        #endregion
    }
}
