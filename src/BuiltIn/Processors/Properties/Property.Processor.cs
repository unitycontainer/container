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


        #region Implementation

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override PropertyInfo[] GetMembers(Type type) => type.GetProperties(BindingFlags);

        public override object? GetValue(ref DependencyInfo dependency, object? data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
