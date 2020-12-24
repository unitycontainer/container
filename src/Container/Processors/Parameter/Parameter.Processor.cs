using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class ParameterProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, ParameterInfo, object[]>
                                                where TMemberInfo : MethodBase
    {
        #region Fields

        /// <summary>
        /// Global singleton containing empty parameter array
        /// </summary>
        protected static object?[] EmptyParametersArray = new object?[0];

        #endregion


        #region Constructors

        static ParameterProcessor()
        {
            GetMemberType = (member) => member.ParameterType;
            GetDeclaringType = (member) => member.Member.DeclaringType!;
        }

        /// <inheritdoc/>
        public ParameterProcessor(IPolicyObservable policies)
            : base(policies) 
            => policies.Set<ImportProvider<ImportInfo, ImportType>>(typeof(ParameterInfo), DefaultImportProvider);

        #endregion
    }
}
