using System;
using System.Reflection;
using Unity.Extension;
using Unity.Injection;

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
        protected SelectorDelegate<InjectionMethodBase<TMemberInfo>, TMemberInfo[], int> IndexFromInjected;

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
        {
            IndexFromInjected = policies.Get<TMemberInfo, SelectorDelegate<InjectionMethodBase<TMemberInfo>, TMemberInfo[], int>>(
                                             OnSelectorChanged)!;
            
            policies.Set<ImportProvider<ImportInfo, ImportType>>(typeof(ParameterInfo), DefaultImportProvider);
        }

        #endregion


        #region Policy Changes

        private void OnSelectorChanged(Type? target, Type type, object? policy)
        {
            if (policy is null) throw new ArgumentNullException(nameof(policy));
            IndexFromInjected = (SelectorDelegate<InjectionMethodBase<TMemberInfo>, TMemberInfo[], int>)policy;
        }

        #endregion
    }
}
