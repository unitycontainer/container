using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, FieldInfo, object>
    {
        #region Constructors

        static FieldProcessor()
        {
            GetMemberType = (member) => member.FieldType;
            GetDeclaringType = (member) => member.DeclaringType!;
        }

        /// <inheritdoc/>
        public FieldProcessor(IPolicyObservable policies)
            : base(policies) 
            // TODO: Move to default extension
            => policies.Set<ImportProvider<ImportInfo, ImportType>>(typeof(FieldInfo), DefaultImportProvider);

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(FieldInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
