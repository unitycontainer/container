using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Exceptions;
using Unity.Policy;
using Unity.Utility;

namespace Unity.Processors
{
    public partial class MethodProcessor : MethodBaseProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(IPolicySet policySet)
            : base(policySet, typeof(InjectionMethodAttribute))
        {
        }

        #endregion


        #region Selection

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetMethodsHierarchical()
                       .Where(c => c.IsStatic == false && c.IsPublic);
#else
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
#endif
        }

        #endregion


        #region Overrides

        protected override void ValidateMember(MethodInfo info)
        {
            var parameters = info.GetParameters();
            if (info.IsGenericMethodDefinition || parameters.Any(param => param.IsOut || param.ParameterType.IsByRef))
            {
                var format = info.IsGenericMethodDefinition
                    ? "The method {1} on type {0} is marked for injection, but it is an open generic method. Injection cannot be performed."
                    : "The method {1} on type {0} has an out parameter. Injection cannot be performed.";

                throw new IllegalInjectionMethodException(string.Format(CultureInfo.CurrentCulture,
                    format, info.DeclaringType.GetTypeInfo().Name, info.Name));
            }

        }

        #endregion
    }
}
