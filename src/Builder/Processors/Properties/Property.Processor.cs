using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class PropertyProcessor<TContext> : MemberProcessor<TContext, PropertyInfo, PropertyInfo, object>
        where TContext : IBuilderContext
    {
        #region Constructors

        /// <inheritdoc/>
        public PropertyProcessor(IPolicies policies)
            : base(policies)
        { }

        #endregion


        #region Implementation

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Execute<TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ValueData data)
        {
            if (!data.IsValue) return;

            descriptor.MemberInfo.SetValue(context.Existing, data.Value);
        }

        /// <inheritdoc/>
        protected override InjectionMember<PropertyInfo, object>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Properties;

        /// <inheritdoc/>
        protected override Type GetMemberType(PropertyInfo info) => info.PropertyType;  

        #endregion
    }
}
