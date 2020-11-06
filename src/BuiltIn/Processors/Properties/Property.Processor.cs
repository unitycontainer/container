using System;
using System.ComponentModel.Composition;
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
            : base(defaults)
        {
        }

        #endregion


        #region Import Info

        protected override bool FillImportInfo(PropertyInfo member, ref ImportInfo<PropertyInfo> info)
        {
            var import = member.GetCustomAttribute<ImportAttribute>(true);
            if (null != import)
            {
                info = new ImportInfo<PropertyInfo>(member, member.PropertyType, import);
                return true;
            }

            var many = member.GetCustomAttribute<ImportManyAttribute>(true);
            if (null != many)
            {
                info = new ImportInfo<PropertyInfo>(member, member.PropertyType, many);
                return true;
            }

            info = new ImportInfo<PropertyInfo>(member, member.PropertyType);
            return false;
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override PropertyInfo[] GetMembers(Type type) => type.GetProperties(BindingFlags);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjected<TMember>(RegistrationManager? registration) 
            where TMember : class => Unsafe.As<TMember>(registration?.Properties);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override Type MemberType(PropertyInfo member) => member.PropertyType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override ImportData AsImportData(PropertyInfo info, object? data) 
            => info.AsImportData(data);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(PropertyInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
