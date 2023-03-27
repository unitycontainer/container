using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public partial class FieldStrategy<TContext> : MemberStrategy<TContext, FieldInfo, FieldInfo, object>
        where TContext : IBuilderContext
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
        protected override void Execute<TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data)
        {
            if (!data.IsValue) return;

            descriptor.MemberInfo.SetValue(context.Existing, data.Value);
        }

        protected override InjectionMember<FieldInfo, object>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Fields;

        #endregion
    }
}
