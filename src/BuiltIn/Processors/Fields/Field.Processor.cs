using System;
using System.ComponentModel.Composition;
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
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override FieldInfo[] GetMembers(Type type) => type.GetFields(BindingFlags);


        public override object? GetValue(ref DependencyInfo dependency, object? data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
