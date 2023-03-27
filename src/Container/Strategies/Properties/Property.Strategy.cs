using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public partial class PropertyStrategy<TContext> : MemberStrategy<TContext, PropertyInfo, PropertyInfo, object>
        where TContext : IBuilderContext
    {
        #region Constructors

        /// <inheritdoc/>
        public PropertyStrategy(IPolicies policies)
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

        protected override InjectionMember<PropertyInfo, object>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Properties;

        #endregion
    }
}
