using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
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

        /// <inheritdoc/>
        public ParameterProcessor(Defaults defaults, Func<Type, TMemberInfo[]> getMembers)
            : base(defaults, (member) => member.ParameterType,
                             (member) => member.Member.DeclaringType!,
                             getMembers, DefaultImportProvider)
        {
        }

        #endregion
    }
}
