using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo> : MemberStrategy<TMemberInfo, ParameterInfo, object[]>
                                               where TMemberInfo : MethodBase
    {
        #region Fields

        /// <summary>
        /// Global singleton containing empty parameter array
        /// </summary>
        protected static object?[] EmptyParametersArray = new object?[0];

        #endregion


        #region Constructors

        static ParameterStrategy()
        {
            GetMemberType = (member) => member.ParameterType;
            GetDeclaringType = (member) => member.Member.DeclaringType!;
        }

        /// <inheritdoc/>
        public ParameterStrategy(IPolicies policies)
            : base(policies) 
            => policies.Set<ImportProvider<ImportInfo, ImportType>>(typeof(ParameterInfo), DefaultImportProvider);

        #endregion
    }
}
